"""Validate the consumer skill in a built NuGet package using only the standard library."""

import argparse
import posixpath
import re
import zipfile
from urllib.parse import unquote, urlsplit


SKILL_NAME = "linq-expressions-deconstruct"
SKILL_ROOT = f"skills/{SKILL_NAME}/"
ENTRY_POINT = f"{SKILL_ROOT}SKILL.md"


def check_frontmatter(markdown):
	lines = markdown.splitlines()
	if not lines or lines[0] != "---":
		raise ValueError("SKILL.md must start with YAML frontmatter")
	try:
		end = lines.index("---", 1)
	except ValueError:
		raise ValueError("SKILL.md frontmatter has no closing delimiter") from None

	# Accept two plain YAML scalars so validation needs no external YAML dependency.
	fields = {}
	for line in lines[1:end]:
		match = re.fullmatch(r"(name|description): ([^\r\n]+)", line)
		if not match or match[1] in fields:
			raise ValueError("Frontmatter requires one plain name and one plain description")
		key, value = match.groups()
		if (value != value.strip() or not value[0].isalpha()
			or re.search(r":(?:\s|$)|\s#", value)
			or value.lower() in {"true", "false", "yes", "no", "on", "off", "null", "y", "n"}):
			raise ValueError(f"Frontmatter {key} must be a plain YAML string on one line")
		fields[key] = value

	if fields.get("name") != SKILL_NAME:
		raise ValueError(f"Frontmatter name must equal folder name {SKILL_NAME}")
	if not 1 <= len(fields.get("description", "")) <= 1024:
		raise ValueError("Frontmatter description must contain 1 to 1024 characters")


def link_destinations(markdown):
	# Code samples are not links. Support inline and reference-style Markdown links.
	markdown = re.sub(r"^(`{3,}|~{3,})[^\n]*\n.*?^\1[^\n]*$", "", markdown, flags=re.M | re.S)
	markdown = re.sub(r"(`+).*?\1", "", markdown, flags=re.S)
	inline = r"\[[^\]]*\]\(\s*(?:<([^>\n]+)>|((?:[^\s()]|\([^\s()]*\))+))"
	reference = r"^ {0,3}\[[^\]\n]+\]:\s*(?:<([^>\n]+)>|(\S+))"
	for pattern in (inline, reference):
		for match in re.finditer(pattern, markdown, flags=re.M):
			yield match[1] or match[2]


def check_package(package_path):
	with zipfile.ZipFile(package_path) as package:
		files = {item.filename for item in package.infolist() if not item.is_dir()}
		if ENTRY_POINT not in files:
			raise ValueError(f"Package is missing {ENTRY_POINT}")
		entry_points = {name for name in files if name.endswith("/SKILL.md") or name == "SKILL.md"}
		if entry_points != {ENTRY_POINT}:
			raise ValueError(f"Unexpected skill entry points: {sorted(entry_points)}")
		if f"{SKILL_ROOT}README.md" in files or "skills/README.md" in files:
			raise ValueError("The maintainer README must not be packed as skill content")

		skill_files = sorted(name for name in files if name.startswith(SKILL_ROOT) and name.endswith(".md"))
		for name in skill_files:
			data = package.read(name)
			if data.startswith(b"\xef\xbb\xbf"):
				raise ValueError(f"{name} must be UTF-8 without BOM")
			markdown = data.decode("utf-8")
			if name == ENTRY_POINT:
				check_frontmatter(markdown)
			for destination in link_destinations(markdown):
				url = urlsplit(destination)
				if url.scheme or url.netloc:
					continue
				path = unquote(url.path)
				if path.startswith("/") or "\\" in path:
					raise ValueError(f"{name} has a nonportable link: {destination}")
				target = posixpath.normpath(posixpath.join(posixpath.dirname(name), path)) if path else name
				if not target.startswith(SKILL_ROOT) or target not in files:
					raise ValueError(f"{name} link must resolve inside the skill folder: {destination}")
		return skill_files


def main():
	parser = argparse.ArgumentParser(description=__doc__)
	parser.add_argument("package", help="Path to the packed Linq.Expressions.Deconstruct .nupkg")
	args = parser.parse_args()
	try:
		files = check_package(args.package)
	except (OSError, ValueError, zipfile.BadZipFile) as error:
		parser.exit(1, f"Skill package check failed: {error}\n")
	print("Skill package check passed:")
	for name in files:
		print(f"  {name}")


if __name__ == "__main__":
	main()

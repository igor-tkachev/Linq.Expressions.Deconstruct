"""Regression checks for validation of the packaged skill."""

import tempfile
import unittest
import zipfile
from pathlib import Path

from check_skill_package import ENTRY_POINT, SKILL_NAME, SKILL_ROOT, check_package


class SkillPackageTests(unittest.TestCase):
	def setUp(self):
		self.directory = tempfile.TemporaryDirectory()
		self.addCleanup(self.directory.cleanup)
		self.package = Path(self.directory.name) / "skill.nupkg"
		self.markdown = f"---\nname: {SKILL_NAME}\ndescription: Match expression trees with this package.\n---\n"

	def pack(self, markdown=None, extra=None):
		with zipfile.ZipFile(self.package, "w") as package:
			package.writestr(ENTRY_POINT, self.markdown if markdown is None else markdown)
			for name, content in (extra or {}).items():
				package.writestr(name, content)
		return self.package

	def test_relative_links_work_from_entry_and_guide(self):
		guide = f"{SKILL_ROOT}docs/patterns.md"
		package = self.pack(self.markdown + "[Patterns](docs/patterns.md#calls)\n", {
			guide: "[Home](../SKILL.md)\n[home]: ../SKILL.md#match-nodes\nSee [home].\n",
		})
		self.assertEqual(check_package(package), [ENTRY_POINT, guide])

	def test_description_length_boundary(self):
		for length in (1, 1024):
			with self.subTest(length=length):
				check_package(self.pack(f"---\nname: {SKILL_NAME}\ndescription: {'x' * length}\n---\n"))

	def test_invalid_frontmatter_is_rejected(self):
		for markdown in (
			self.markdown.replace(SKILL_NAME, "another-skill"),
			self.markdown.replace("description:", "missing:"),
			f"---\nname: {SKILL_NAME}\ndescription: {'x' * 1025}\n---\n",
			self.markdown.replace("Match expression trees with this package.", ""),
			self.markdown.replace("Match expression", "Match: expression"),
			self.markdown.replace("Match expression", "Match #expression"),
			self.markdown.replace("Match expression", "Match\nexpression"),
			self.markdown.replace("description:", f"name: {SKILL_NAME}\ndescription:"),
			self.markdown.removesuffix("---\n"),
			"\ufeff" + self.markdown,
		):
			with self.subTest(markdown=markdown):
				with self.assertRaises(ValueError):
					check_package(self.pack(markdown))

	def test_missing_entry_point_is_rejected(self):
		with zipfile.ZipFile(self.package, "w"):
			pass
		with self.assertRaisesRegex(ValueError, "missing"):
			check_package(self.package)

	def test_missing_or_escaping_links_are_rejected(self):
		for destination in ("docs/missing.md", "../outside.md", "%2e%2e/outside.md", "/SKILL.md", "docs\\patterns.md"):
			with self.subTest(destination=destination):
				for link in (f"[Guide]({destination})", f"[guide]: {destination}\nSee [guide]."):
					with self.assertRaises(ValueError):
						check_package(self.pack(self.markdown + link, {"skills/outside.md": "Outside"}))

	def test_links_in_samples_are_ignored(self):
		check_package(self.pack(self.markdown + "`[sample](missing.md)`\n\n```markdown\n[sample](missing.md)\n```\n"))

	def test_links_in_guides_are_checked(self):
		with self.assertRaisesRegex(ValueError, "inside the skill folder"):
			check_package(self.pack(extra={
				f"{SKILL_ROOT}docs/patterns.md": "[Package readme](../../../README.md)",
				"README.md": "Package readme",
			}))

	def test_external_and_fragment_links_are_allowed(self):
		check_package(self.pack(self.markdown + "[Here](#match) [Web](https://example.com/guide.md)\n"))

	def test_duplicate_skill_and_maintainer_readme_are_rejected(self):
		for name in ("SKILL.md", "skills/other/SKILL.md", "skills/README.md", f"{SKILL_ROOT}README.md"):
			with self.subTest(name=name):
				with self.assertRaises(ValueError):
					check_package(self.pack(extra={name: "Maintainer file"}))


if __name__ == "__main__":
	unittest.main()

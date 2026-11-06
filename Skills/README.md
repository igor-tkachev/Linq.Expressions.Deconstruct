# Package skills for maintainers

`linq-expressions-deconstruct/SKILL.md` is the consumer entry point. Its optional
task guides live in `linq-expressions-deconstruct/docs/`. The library project
packs only those Markdown files under `skills/linq-expressions-deconstruct/`.
This README is for maintainers and is not packed. There is no root-level skill
copy; the repository's root README remains the NuGet package readme.

Consumers read the skill from
`<global-packages>/linq.expressions.deconstruct/<version>/skills/linq-expressions-deconstruct/`
or copy the whole skill folder into `.agents/skills/linq-expressions-deconstruct/`
or `.claude/skills/linq-expressions-deconstruct/`. Only the main
`Linq.Expressions.Deconstruct` package carries this skill. Do not add another
package carrying the same skill name. Do not create or write
`.dotnet-package-skills.json`; that beta-format ownership file belongs to the
`dotnet-package-skills` tool.

## Rules for skill content

- The skill works in all consumer locations above. Links between skill files
  are relative and stay inside `linq-expressions-deconstruct/`. Do not link to
  package-root paths or repository paths such as `Source/...`. The entry point
  states the availability of `Linq.Expressions.Deconstruct.xml` once; guides
  refer to that file by name if needed.
- Frontmatter `name` equals the folder name, `linq-expressions-deconstruct`.
  `description` explains coverage and when to use the skill. Keep it on one
  line, at most 1024 characters, without `: ` or ` #` sequences. Use plain YAML
  scalars for both fields; the package check validates this restricted format.
- Write for a reader opening one guide for one task. Each guide starts with a
  one-line pointer to `SKILL.md` and a "You are here if" block. State rules
  plainly; avoid banners, capitals and "must read" instructions.
- Text written into user code, including comments and names, is ordinary code
  text with no notes about the agent that wrote it.
- Keep guidance versioned with the code it describes. A change to a documented
  public API, pattern shape, traversal behavior or default updates the guide
  in the same PR. Check generated `Deconstruct` overloads as well as handwritten
  ones, and verify examples against the tests.
- Keep maintainer prompts, roadmaps, test prompts and internal notes outside
  `linq-expressions-deconstruct/`; every file inside it ships to consumers.
- Skill files are UTF-8 without BOM. Line endings follow `.gitattributes` when
  present. Use repository-style CRLF here; the Markdown editor override keeps
  skill sources free of BOMs.

## Check the package

CI packs the library and runs `Tests/check_skill_package.py` on the resulting
`.nupkg`. The check reads the archive itself, validates frontmatter and ensures
relative Markdown links resolve to files inside the packaged skill folder.
Use inline or reference-style Markdown links, including optional fragments.

```bash
export MSBUILDDISABLENODEREUSE=1
dotnet pack Source/Linq.Expressions.Deconstruct.csproj -c Release -nodeReuse:false -o artifacts/skills
python3 Tests/check_skill_package.py artifacts/skills/Linq.Expressions.Deconstruct.1.1.0.nupkg
```

Adjust the package filename when the version changes. Keep packing and validation
in CI when changing the project layout or skill files.

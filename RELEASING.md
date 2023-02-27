# Releasing Process

- Update `CHANGELOG.md` with the changes since the last release. Consider automating with a command such as these two:
  - `git log $(git describe --tags --abbrev=0)..HEAD --no-merges --oneline > new-in-this-release.log`
  - `git log --pretty='%C(green)%d%Creset- %s | [%an](https://github.com/)'`
- Update VersionPrefix (and VersionSuffix if necessary) in the `*.csproj` files
  - If updating the OTel SDK, update the OTLP version in `HoneycombOptions.cs`
- Commit changes, push, and open a release preparation pull request for review.
- Once the pull request is merged, fetch the updated `main` branch.
- Apply a tag for the new version on the merged commit (e.g. `git tag -a v1.2.1v/0.26.1-beta -m "v1.2.1v/0.26.1-beta"`)
- Push the tag upstream (this will kick off the release pipeline in CI) e.g. `git push origin v1.2.1v/0.26.1-beta`
- Ensure that there is a draft GitHub release created as part of CI publish steps (this will also publish to Nuget).
- Click "generate release notes" in Github for full changelog notes and any new contributors
- Publish the Github draft release - if it is a prerelease (e.g. beta) click the prerelease checkbox.

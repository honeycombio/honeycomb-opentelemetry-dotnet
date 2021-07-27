# Releasing Process

1. Update changelog
1. Update VersionPrefix (and VersionSuffix if necessary) in the csproj file
1. Open a PR with the above, and merge that into main
1. Tag the merged commit with the new version (e.g. `v0.10.0-alpha`)
1. Push the tag upstream (this will kick off the release pipeline in CI)
1. Once the CI is done, promote the GitHub release from draft to release
# Releasing Process

1. Update changelog
2. Update VersionPrefix (and VersionSuffix if necessary) in the csproj file
    - If updating the OTel SDK, update the OTLP version in ``
3. Open a PR with the above, and merge that into main
4. Tag the merged commit with the new version (e.g. `v0.10.0-alpha`)
5. Push the tag upstream (this will kick off the release pipeline in CI)
6. Once the CI is done, publish the GitHub draft release as pre-release through GitHub UI
7. Update public docs with the new version
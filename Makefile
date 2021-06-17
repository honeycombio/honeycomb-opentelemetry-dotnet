# default location for local nuget package source
NUGET_PACKAGES_LOCAL ?= ${HOME}/.nuget/local

build:
	dotnet build

test: build
	dotnet test --no-build

install:
	@[ -d ${NUGET_PACKAGES_LOCAL} ] || mkdir -p ${NUGET_PACKAGES_LOCAL}
	@dotnet nuget list source | grep -q '${NUGET_PACKAGES_LOCAL}' || dotnet nuget add source -n local ${NUGET_PACKAGES_LOCAL}
	@echo "Publishing nuget package(s) to: ${NUGET_PACKAGES_LOCAL}\n"
	@dotnet pack -c release -o ${NUGET_PACKAGES_LOCAL} --include-symbols

.PHONY: build

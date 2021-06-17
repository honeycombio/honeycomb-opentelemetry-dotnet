# default locaion for local nuget package source
NUGET_PACKAGES_LOCAL ?= ${HOME}/.nuget/local

test:
	dotnet test

install:
	@[ -d ${NUGET_PACKAGES_LOCAL} ] || mkdir -p ${NUGET_PACKAGES_LOCAL}
	@echo "~~~~~\nPublishing nuget package(s) to: ${NUGET_PACKAGES_LOCAL}\n~~~~~\n\n"
	@dotnet pack -c release -o ${NUGET_PACKAGES_LOCAL}

.PHONY: test

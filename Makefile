# default location for local nuget package source
NUGET_PACKAGES_LOCAL ?= ${HOME}/.nuget/local

build:
	dotnet build

test: build
	dotnet test --no-build

clean:
	rm -rf ./examples/aspnetcore/bin/*
	rm -rf ./examples/aspnetcore/obj/*
	rm -rf ./test/Honeycomb.OpenTelemetry.Tests/bin/*
	rm -rf ./test/Honeycomb.OpenTelemetry.Tests/obj/*
	rm -rf ./src/Honeycomb.OpenTelemetry/bin/*
	rm -rf ./src/Honeycomb.OpenTelemetry/obj/*
	dotnet clean

smoke:
	@echo ""
	@echo "+++ Placeholder for Smoking all the tests."
	@echo ""
	cd smoke-tests && docker-compose up -d --build && docker-compose down --volumes

unsmoke:
	@echo ""
	@echo "+++ Spinning down the smokers."
	@echo ""
	cd smoke-tests && docker-compose down --volumes

resmoke: unsmoke smoke

${NUGET_PACKAGES_LOCAL}:
	@mkdir -p ${NUGET_PACKAGES_LOCAL}

local_nuget_source_registered: ${NUGET_PACKAGES_LOCAL}
	@dotnet nuget list source | grep -q '${NUGET_PACKAGES_LOCAL}' || dotnet nuget add source -n local ${NUGET_PACKAGES_LOCAL}

publish_local: local_nuget_source_registered
	@echo "Publishing nuget package(s) to: ${NUGET_PACKAGES_LOCAL}\n"
	@dotnet pack -c release -o ${NUGET_PACKAGES_LOCAL} -p:signed=false

.PHONY: build test clean smoke unsmoke resmoke local_nuget_source_registered publish_local

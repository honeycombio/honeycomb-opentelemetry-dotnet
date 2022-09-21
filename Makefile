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
	rm -rf ./smoke-tests/report.*
	rm -rf ./smoke-tests/collector/data.json
	rm -rf ./smoke-tests/collector/data-results/*.json
	dotnet clean

smoke-tests/collector/data.json:
	@echo ""
	@echo "+++ Zhuzhing smoke test's Collector data.json"
	@touch $@ && chmod o+w $@

smoke-sdk-grpc: smoke-tests/collector/data.json
	@echo ""
	@echo "+++ Running gRPC smoke tests."
	@echo ""
	cd smoke-tests && bats ./smoke-sdk-grpc.bats --report-formatter junit --output ./

smoke-sdk-http: smoke-tests/collector/data.json
	@echo ""
	@echo "+++ Running HTTP smoke tests."
	@echo ""
	cd smoke-tests && bats ./smoke-sdk-http.bats --report-formatter junit --output ./

smoke-sdk: smoke-sdk-grpc smoke-sdk-http

smoke: smoke-sdk

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

.PHONY: build test clean smoke unsmoke resmoke local_nuget_source_registered publish_local smoke-sdk-grpc smoke-sdk

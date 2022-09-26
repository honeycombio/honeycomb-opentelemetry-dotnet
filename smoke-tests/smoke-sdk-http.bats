#!/usr/bin/env bats

load test_helpers/utilities

CONTAINER_NAME="app-sdk-http"
OTEL_SERVICE_NAME="aspnetcore-example"

setup_file() {
	echo "# 🚧" >&3
	docker-compose up --detach collector ${CONTAINER_NAME}
	wait_for_ready_app ${CONTAINER_NAME}
	curl --silent "https://localhost:5001/weatherforecast"
	wait_for_traces
}

teardown_file() {
	cp collector/data.json collector/data-results/data-${CONTAINER_NAME}.json
	docker-compose stop ${CONTAINER_NAME}
	docker-compose restart collector
	wait_for_flush
}

# TESTS

@test "Manual instrumentation produces span with name of span" {
	result=$(span_names_for ${OTEL_SERVICE_NAME})
	assert_equal "$result" '"sleep"'
}

@test "Manual instrumentation adds custom attribute" {
	result=$(span_attributes_for ${OTEL_SERVICE_NAME} | jq "select(.key == \"delay_ms\").value.intValue")
	assert_equal "$result" '"100"'
}

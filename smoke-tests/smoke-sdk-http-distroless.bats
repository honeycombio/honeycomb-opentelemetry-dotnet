#!/usr/bin/env bats

load test_helpers/utilities

CONTAINER_NAME="app-sdk-http-distroless"
OTEL_SERVICE_NAME="aspnetcore-example"
METRICS_DATASET="bogus_dataset"

setup_file() {
	echo "# 🚧" >&3
	docker-compose up --build --detach collector ${CONTAINER_NAME}
	wait_for_ready_app ${CONTAINER_NAME}
	curl --silent "http://localhost:5001/weatherforecast"
	wait_for_traces
    wait_for_metrics 15
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

@test "Manual instrumentation produces metrics" {
    result=$(metric_names_for ${METRICS_DATASET})
    assert_equal "$result" '"sheep"'
}

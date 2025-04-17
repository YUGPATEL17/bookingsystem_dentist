SOLUTION = DentistAppointmentManagementSystem.sln
PROJECT  = src/DentistAppointmentManagementSystem/DentistAppointmentManagementSystem.csproj

.PHONY: all restore build test run clean

all: restore build test

restore:
	dotnet restore $(SOLUTION)

build: restore
	dotnet build $(SOLUTION) -c Release

test: build
	dotnet test $(SOLUTION) -c Release --no-build \
	    --filter "TestCategory!=Integration"


run: build
	dotnet run --project $(PROJECT) -c Release -- $(ARGS)

clean:
	dotnet clean $(SOLUTION)

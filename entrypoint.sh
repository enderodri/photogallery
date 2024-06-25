#!/bin/bash

set -e

# Run migrations
dotnet ef database update

# Start the application
exec dotnet PhotoGallery.dll

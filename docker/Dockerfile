# Use a minimal Windows base image
FROM mcr.microsoft.com/windows/servercore:ltsc2022

# Set the working directory
WORKDIR /app

# Copy the self-contained executable and its dependencies
# Make sure to build your self-contained executable first and place it in the same directory as this Dockerfile
COPY ./deploy/jformat-win-x64-sc.exe ./
COPY ./deploy/jformat.dll ./
# Copy any other dependencies your application might need
# For example, if you have additional DLLs, copy them here
# COPY ./deploy/YourDependency.dll ./

# Set the entry point for the application
ENTRYPOINT ["jformat-win-x64-sc.exe"]
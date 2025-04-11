# Use a minimal Windows base image
FROM mcr.microsoft.com/windows/servercore:ltsc2019

# Set the working directory
WORKDIR /dockerapp

# Copy the self-contained executable and its dependencies
# Make sure to build your self-contained executable first and place it in the same directory as this Dockerfile

# should work
COPY ./deploy/jformat-win-x64-sc.exe ./
# shouldn't work
COPY ./deploy/jformat-win-x64-dep.exe ./


COPY ./deploy/ex2.json ./


# Set the entry point for the application
#ENTRYPOINT ["jformat-win-x64-sc.exe"]

# interactive instead
CMD ["cmd.exe"]

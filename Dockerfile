FROM node:20-alpine AS node
RUN apk add --no-cache util-linux
WORKDIR /build/

# Copy source files
ADD src/Isotope/Isotope/ .

# Install dependencies for both apps
RUN cd Areas/Admin/ClientApp && npm ci
RUN cd Areas/Front/ClientApp && npm ci

# Build both frontends
RUN cd Areas/Admin/ClientApp && npm run build
RUN cd Areas/Front/ClientApp && npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS net-builder
WORKDIR /build
ADD src/Isotope/Isotope.sln .
ADD src/Isotope/Isotope/Isotope.csproj Isotope/

RUN dotnet restore
COPY --from=node /build Isotope/

RUN dotnet publish --output ../out/ --configuration Release --runtime linux-musl-x64 --self-contained true Isotope/Isotope.csproj

FROM alpine:latest

RUN apk add --no-cache libintl icu && \
    apk add --no-cache libgdiplus --repository=http://dl-cdn.alpinelinux.org/alpine/edge/testing/

WORKDIR /app
COPY --from=net-builder /out .

RUN chmod +x /app/Isotope

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

ENTRYPOINT ["/app/Isotope"]

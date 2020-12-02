FROM node:lts-alpine as node
RUN apk add --no-cache util-linux
WORKDIR /build/

ADD src/Isotope/Isotope/package.json .
ADD src/Isotope/Isotope/package-lock.json .
RUN npm ci

ADD src/Isotope/Isotope/ .
RUN find .
RUN npm run build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as net-builder
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

ENTRYPOINT ["/app/Isotope"]

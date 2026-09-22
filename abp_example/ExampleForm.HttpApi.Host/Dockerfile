FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80
RUN sed -i 's/dl-cdn.alpinelinux.org/mirrors.aliyun.com/g' /etc/apk/repositories
RUN apk add --no-cache tzdata
ENV TZ="Asia/Shanghai"
ENV LANG C.UTF-8

FROM base AS final
WORKDIR /app
EXPOSE 80
COPY . .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ExampleForm.HttpApi.Host.dll"]
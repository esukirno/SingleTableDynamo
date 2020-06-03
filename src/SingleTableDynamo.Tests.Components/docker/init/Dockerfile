FROM mcr.microsoft.com/dotnet/core/sdk:2.1-alpine

RUN apk --no-cache update && \
    apk add --no-cache bash && \
    apk add --no-cache zip && \
    apk --no-cache add python py-pip py-setuptools ca-certificates curl groff less && \
    pip --no-cache-dir install awscli==1.17.6 && \
    rm -rf /var/cache/apk/*

COPY . /usr/app
RUN chmod +x /usr/app/wait_for_it.sh
RUN chmod +x /usr/app/init.sh

FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
COPY yarn.lock ./

RUN yarn config set network-timeout 600000 -g \
 && yarn install --frozen-lockfile --non-interactive --retry 5

COPY . .
RUN yarn build:prod

FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY dynamic-env.json /usr/share/nginx/html/dynamic-env.json
COPY --from=build /app/dist/WorkShopManagement /usr/share/nginx/html
EXPOSE 8080

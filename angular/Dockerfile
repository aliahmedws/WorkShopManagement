FROM node:20-alpine AS build

WORKDIR /app
COPY package*.json /app/
RUN yarn

COPY . /app
RUN yarn build:prod

FROM nginx:alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf

# Remove default nginx website content
RUN rm -rf /usr/share/nginx/html/*

# Copy your build output into the web root (note trailing slash)
COPY --from=build /app/dist/WorkShopManagement/browser/ /usr/share/nginx/html/

# Copy dynamic env config
COPY dynamic-env.json /usr/share/nginx/html/dynamic-env.json

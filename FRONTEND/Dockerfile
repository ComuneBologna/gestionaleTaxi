FROM node:12.20-alpine AS build
WORKDIR /usr/src/compilation
COPY . .
RUN npm install && npm run build-pipeline
FROM nginx:1.17.1-alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /usr/src/compilation/dist /usr/share/nginx/html
RUN rm -rf /usr/src/compilation/*


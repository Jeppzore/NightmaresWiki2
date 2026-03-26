FROM node:22-alpine AS build
WORKDIR /app

COPY src/NightmaresWiki.Frontend/package.json ./
COPY src/NightmaresWiki.Frontend/tsconfig.json ./
COPY src/NightmaresWiki.Frontend/tsconfig.app.json ./
COPY src/NightmaresWiki.Frontend/tsconfig.node.json ./
COPY src/NightmaresWiki.Frontend/vite.config.ts ./
COPY src/NightmaresWiki.Frontend/index.html ./
COPY src/NightmaresWiki.Frontend/src ./src

RUN npm install

ARG VITE_API_BASE_URL=http://localhost:8080
ENV VITE_API_BASE_URL=${VITE_API_BASE_URL}

RUN npm run build

FROM nginx:1.27-alpine AS final
COPY src/NightmaresWiki.Frontend/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

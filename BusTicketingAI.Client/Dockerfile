# 1. Aşama: Node.js ile Angular projesini derleme
FROM node:22-alpine AS build
WORKDIR /app

# Bağımlılıkları kopyala ve yükle
COPY package.json package-lock.json ./
RUN npm install --legacy-peer-deps

# Tüm kodları kopyala ve production için build et
COPY . .
RUN npm run build --configuration=production

# 2. Aşama: Nginx ile ayağa kaldırma
FROM nginx:alpine
COPY --from=build /app/dist/BusTicketingAI.Client/browser /usr/share/nginx/html
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
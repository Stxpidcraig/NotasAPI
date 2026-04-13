## Ejecutar con Docker

1. Construir la imagen:
docker build -t notasapi .

3. Ejecutar el contenedor:
docker run --rm -p 8080:8080 --name notasapi-container notasapi

4. Abrir Swagger:
http://localhost:8080/swagger

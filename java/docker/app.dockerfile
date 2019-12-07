FROM gradle:6.0.1-jdk11 as builder
COPY --chown=gradle:gradle . /home/gradle/
WORKDIR /home/gradle
RUN gradle build

FROM azul/zulu-openjdk-alpine:11.0.5-jre as runtime

COPY --from=builder /home/gradle/app/build/libs/app-1.0.0.jar ./app/app.jar
CMD java -jar ./app/app.jar
EXPOSE 4567
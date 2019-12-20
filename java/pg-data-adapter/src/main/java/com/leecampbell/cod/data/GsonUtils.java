package com.leecampbell.cod.data;

import com.google.gson.*;

import java.lang.reflect.Type;
import java.time.LocalDate;
import java.time.ZoneOffset;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.time.temporal.ChronoUnit;

/**
 * Utility class to provide simple access to serialise and deserialize JSON via a standardised
 * instance of a GSON object.
 *
 * <p>This uses standardised TypeAdapters to ensure consistent behaviour across VBO repositories.
 */
final class GsonUtils {

    private static final Gson gson =
            new GsonBuilder()
                    .setDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
                    .registerTypeAdapter(LocalDate.class, new LocalDateAdapter())
                    .registerTypeAdapter(ZonedDateTime.class, new ZonedDateTimeAdapter())
                    .create();

    private GsonUtils() {
    }

    /**
     * Deserializes the provided JSON string into the specified type.
     *
     * @param data  the JSON string to deserialize.
     * @param clazz the target class to deserialize to
     * @param <T>   the return type
     * @return an instance of {@code clazz}
     * @throws JsonSyntaxException if the content can not be deserialized to the target type
     */
    public static <T> T deserialize(String data, Class<T> clazz) {
        return gson.fromJson(data, clazz);
    }


    /**
     * Serializes the provided object to JSON
     *
     * @param obj the source object
     * @return the JSON string representation
     */
    public static String serialize(Object obj) {
        return gson.toJson(obj);
    }

    private static class LocalDateAdapter
            implements JsonSerializer<LocalDate>, JsonDeserializer<LocalDate> {

        @Override
        public JsonElement serialize(LocalDate date, Type typeOfSrc, JsonSerializationContext context) {
            return new JsonPrimitive(date.format(DateTimeFormatter.ISO_LOCAL_DATE)); // "yyyy-mm-dd"
        }

        @Override
        public LocalDate deserialize(
                JsonElement jsonElement, Type type, JsonDeserializationContext jsonDeserializationContext)
                throws JsonParseException {
            return DateTimeFormatter.ISO_LOCAL_DATE.parse(jsonElement.getAsString(), LocalDate::from);
        }
    }

    private static class ZonedDateTimeAdapter
            implements JsonSerializer<ZonedDateTime>, JsonDeserializer<ZonedDateTime> {

        @Override
        public JsonElement serialize(
                ZonedDateTime zonedDateTime, Type type, JsonSerializationContext jsonSerializationContext) {
            DateTimeFormatter fmt = DateTimeFormatter.ISO_OFFSET_DATE_TIME.withZone(ZoneOffset.UTC);
            return new JsonPrimitive(fmt.format(zonedDateTime.truncatedTo(ChronoUnit.NANOS)));
        }

        @Override
        public ZonedDateTime deserialize(
                JsonElement jsonElement, Type type, JsonDeserializationContext jsonDeserializationContext)
                throws JsonParseException {
            return ZonedDateTime.from(
                    DateTimeFormatter.ISO_OFFSET_DATE_TIME
                            .withZone(ZoneOffset.UTC)
                            .parse(jsonElement.getAsString()));
        }
    }
}
package net.pic4pic.ginger.utils;

import java.lang.reflect.Type;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonParseException;

import net.pic4pic.ginger.entities.IntegerEnum;

public class JsonEnumDeserializer<T extends Enum<T> & IntegerEnum> implements JsonDeserializer<T>{
	
	@SuppressWarnings("unchecked")
	public T deserialize(JsonElement json, Type typeOfT, JsonDeserializationContext context) throws JsonParseException {
		int intValue = json.getAsInt();
		Class<T> classType = ((Class<T>)typeOfT);
		T[] enumConstants = classType.getEnumConstants();
		for(IntegerEnum enumConstant : enumConstants) {
			if(enumConstant.getIntValue() == intValue) {
				return (T) enumConstant;
			}
		}
		
		return null;
	} 
}

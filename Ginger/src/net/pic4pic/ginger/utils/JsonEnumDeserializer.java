package net.pic4pic.ginger.utils;

import java.lang.reflect.Type;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonParseException;
import com.google.gson.JsonPrimitive;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;

import net.pic4pic.ginger.entities.IntegerEnum;

public class JsonEnumDeserializer<T extends Enum<T> & IntegerEnum> implements JsonDeserializer<T>, JsonSerializer<T>{
	
	@SuppressWarnings("unchecked")
	public T deserialize(JsonElement json, Type typeOfT, JsonDeserializationContext context) throws JsonParseException {
		
		// get value from jSON element
		int intValue = json.getAsInt();
		
		// create integerEnum 
		Class<T> classType = ((Class<T>)typeOfT);
		
		// get all possible enumeration values
		T[] enumConstants = classType.getEnumConstants();
		
		// pick the matching one
		for(IntegerEnum enumConstant : enumConstants) {
			
			// check
			if(enumConstant.getIntValue() == intValue) {
				
				// return matched enumeration constant
				return (T) enumConstant;
			}
		}
		
		// couldn't found...
		return null;
	}

	@Override
	public JsonElement serialize(T enumObject, Type typeOfT, JsonSerializationContext context) {
		return new JsonPrimitive(enumObject.getIntValue());
	}
}

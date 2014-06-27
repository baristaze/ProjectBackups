package net.pic4pic.ginger.utils;

import java.lang.reflect.Type;
import java.util.Date;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonParseException;
import com.google.gson.JsonPrimitive;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;

// Examples
// 
//	\/Date(1391918153637)\/					// No time-zone info
//	\/Date(1391918153637-0800)\/			// With time-zone info; i.e. GMT-8 in this example
//	\/Date(-62135596800000+0000)\/			// UTC time
//
//		Strategies:
//		1) Ignore UTC... It might be mis-leading... It needs to be handled via client and server...
//		2)
//			omit first 6 chars which are "\/Date("
//			omit last 2 chars which are ")\/"
// 			omit anything after + or -
public class JsonDateDeserializer implements JsonDeserializer<Date>, JsonSerializer<Date>{
	
	public Date deserialize(JsonElement json, Type typeOfT, JsonDeserializationContext context) throws JsonParseException {

		// get everything as a string 
		String s = json.getAsJsonPrimitive().getAsString();
		String original = s;

		// trim "\/Date(" and ")\/" parts
		s = s.substring(6, s.length() - 2);
		
		// trim anything after "+"
		int signIndex = s.indexOf("+");
		if(signIndex > 0){ // not >= !!!!
			s = s.substring(0, signIndex);
		}
		
		// trim anything after "-". Notice that "-" can be at the beginning which refers to a null-date 
		signIndex = s.indexOf("-");
		if(signIndex > 0){ // not >= !!!!
			s = s.substring(0, signIndex);
		}
		
		// convert to long
		long l = 0;
		try {
			l = Long.parseLong(s);
		}
		catch(NumberFormatException e){
			MyLog.e("Exception", e.toString());
			MyLog.e("JSON", "Parsing JSON DateTime to Java Date failed: " + original);
		}
		
		if(l > 0){
			return new Date(l);
		}
		else{
			return null;
		}
	} 
	
	@Override
	public JsonElement serialize(Date date, Type typeOfDate, JsonSerializationContext context) {
		return new JsonPrimitive("/Date(" + date.getTime() + ")/");
	}
}
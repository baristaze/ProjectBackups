package net.pic4pic.ginger.utils;

import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.UnsupportedEncodingException;
import java.util.Date;
import java.util.Scanner;

import net.pic4pic.ginger.GingerException;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.client.HttpClient;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.params.BasicHttpParams;
import org.apache.http.params.HttpConnectionParams;
import org.apache.http.params.HttpParams;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

public class GingerNetUtils {

	public static HttpClient getHttpClient() {
		return getHttpClient(10);
	}

	public static HttpClient getHttpClient(int timeoutSeconds) {
		return getHttpClient(timeoutSeconds, timeoutSeconds);
	}

	public static HttpClient getHttpClient(int connectionTimeoutSeconds,
			int dataTimeoutSeconds) {

		// Set the timeout in milliseconds until a connection is established.
		HttpParams httpParameters = new BasicHttpParams();
		HttpConnectionParams.setConnectionTimeout(httpParameters,
				connectionTimeoutSeconds * 1000);

		// Set the default socket timeout (SO_TIMEOUT)
		// in milliseconds which is the timeout for waiting for data.
		HttpConnectionParams.setSoTimeout(httpParameters,
				dataTimeoutSeconds * 1000);
		return new DefaultHttpClient(httpParameters);
	}

	public static HttpClient getDefaultHttpClientForFiles() {
		return getHttpClient(10, 90);
	}

	public static <T> T getJsonObjectFrom(HttpResponse response,
			Class<T> theClass) throws GingerException {

		if (response == null) {
			throw new GingerException("Response is null");
		}

		if (response.getStatusLine() == null) {
			throw new GingerException(
					"Status information on the Response is null");
		}

		int resultCode = response.getStatusLine().getStatusCode();
		if (resultCode != HttpStatus.SC_OK) {
			throw new GingerException("HTTP Operation has failed. HTTP Result code is: " + resultCode);
		}

		HttpEntity httpEntity = response.getEntity();
		if (httpEntity == null) {
			throw new GingerException("HttpEntity is null");
		}

		InputStream entityContent;
		try {
			entityContent = httpEntity.getContent();
		} 
		catch (IllegalStateException e) {
			e.printStackTrace();
			String err = "Unknown IllegalStateException when getting the content of HTTP Response.";
			throw new GingerException(err, e);
		} 
		catch (IOException e) {
			e.printStackTrace();
			String err = "Unknown IOException when getting the content of HTTP Response.";
			throw new GingerException(err, e);
		}

		if (entityContent == null) {
			throw new GingerException("HTTP EntityContent is null");
		}

		String encoding = "UTF-8";
		InputStreamReader stream;
		try {
			stream = new InputStreamReader(entityContent, encoding);
		} 
		catch (UnsupportedEncodingException e) {
			e.printStackTrace();
			String err = "Unsupported Encoding: " + encoding + " when reading the content of HTTP Response.";
			throw new GingerException(err, e);
		}

		StringBuilder json = new StringBuilder();
		Scanner scanner = new Scanner(stream);
		while (scanner.hasNextLine()) {
			json.append(scanner.nextLine());
			json.append("\n");
		}

		Gson gson = createGson();
		return gson.fromJson(json.toString(), theClass);
	}

	public static Gson createGson() {

		// Creates the json object which will manage the information received
		GsonBuilder builder = new GsonBuilder();

		// Register an adapter to manage the date types as long values
		builder.registerTypeAdapter(Date.class, new JsonDateDeserializer());

		return builder.create();
	}
}

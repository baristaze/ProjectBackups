package net.pic4pic.ginger.entities;

import com.google.gson.annotations.SerializedName;

public class SimpleRequest<T> extends BaseRequest {
	
	@SerializedName("Data")
	protected T data;

	/**
	 * @return the data
	 */
	public T getData() {
		return data;
	}

	/**
	 * @param data the data to set
	 */
	public void setData(T data) {
		this.data = data;
	}
}

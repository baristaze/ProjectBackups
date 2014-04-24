package net.pic4pic.ginger.entities;

import java.util.ArrayList;
import com.google.gson.annotations.SerializedName;

public class SimpleListResponse<T> extends BaseResponse {

	private static final long serialVersionUID = 1;
	
	@SerializedName("Items")
	protected ArrayList<T> items = new ArrayList<T>();

	/**
	 * @return the items
	 */
	public ArrayList<T> getItems() {
		return items;
	}
}

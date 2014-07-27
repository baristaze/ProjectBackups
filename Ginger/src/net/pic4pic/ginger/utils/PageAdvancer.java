package net.pic4pic.ginger.utils;

import net.pic4pic.ginger.entities.UserResponse;

public interface PageAdvancer {
	public void moveToNextPage(int data);
	public void moveToPreviousPage();
	public void moveToLastPage(UserResponse response, boolean backEnabled);
}

package net.pic4pic.ginger;

public class GingerException extends Exception {

	private static final long serialVersionUID = 1L;

	public GingerException(String errorMessage){
		this(errorMessage, null);
	}
	
	public GingerException(String errorMessage, Throwable innerException){
		super(errorMessage, innerException);
	}	
}

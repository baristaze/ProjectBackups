package net.pic4pic.ginger.entities;

public enum AppStoreType implements IntegerEnum {
	
	Unknown(0),
    GooglePlay(1);
    
    private final int value;
	
	private AppStoreType(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
}

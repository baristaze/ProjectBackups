package net.pic4pic.ginger.service;

public class Service {
	
	private static IService instance;
	
	public synchronized static IService getInstance() {
		
        if (instance == null) {
        	instance = new ServiceProxy();
        }

        return instance;
    }
}

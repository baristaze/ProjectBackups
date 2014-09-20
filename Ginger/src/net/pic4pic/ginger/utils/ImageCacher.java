package net.pic4pic.ginger.utils;

import java.util.Collections;
import java.util.Map;
import java.util.UUID;

import android.graphics.Bitmap;

public class ImageCacher implements LRUCache.EntryRemoveListener<UUID, Bitmap>{
	
	// constants
	public static final UUID UserProfileFullPictureId = UUID.randomUUID();
	
	// private properties
	private Map<UUID, Bitmap> cache;
	
	// constructor
	private ImageCacher(int capacity){		
		// last parameter makes it get sorted by access-order		
		this.cache = Collections.synchronizedMap(new LRUCache<UUID, Bitmap>(capacity, true, this));
	}
	
	// public methods
	
	public void put(UUID id, Bitmap image){
		this.cache.put(id, image);
	}
	
	public boolean exists(UUID id){
		return this.cache.containsKey(id);
	}
	
	public Bitmap get(UUID id){
		// below method returns null if item doesn't exists
		return this.cache.get(id);
	}
	
	public void clear(){
		
		// not yet. it brings more problems... 
		// e.g. Existing ImageViews' onDraw methods throws exception
		// stating that image bitmap has been recycled already.
		
		/*	
		MyLog.bag().w("Clearing image cache");
		
		// do not call iterate via 'this.cache.keySet()'
		// because you will get a ConcurrentModificationException 
		// when you make a this.cache.get(key) call within the for loop. 
		for(Bitmap bitmap : this.cache.values()){
			bitmap.recycle();
			bitmap = null;
		}		
		
		this.cache.clear();
		*/
	}
	
	// statics
	
	private static ImageCacher instance;
	private static int cacheCapacity = 100;	
	public static synchronized void PreInit(int capacity) throws Exception {
		
		if(ImageCacher.instance != null){
			throw new Exception("Image Cache is already initialized");
		}
		
		if(capacity <= 0){
			throw new Exception("Capacity cannot be 0 or smaller");
		}
		
		ImageCacher.cacheCapacity = capacity;
	}
	
	public static synchronized ImageCacher Instance(){		
		
		if(ImageCacher.instance == null){
			ImageCacher.instance = new ImageCacher(ImageCacher.cacheCapacity);
		}
		
		return ImageCacher.instance;
	}

	@Override
	public void onRemovingEntry(UUID key, Bitmap bitmap) {
		
		// not yet. it brings more problems... 
		// e.g. Existing ImageViews' onDraw methods throws exception
		// stating that image bitmap has been recycled already.
		
		/*
		bitmap.recycle();
		bitmap = null;
		*/
	}
}

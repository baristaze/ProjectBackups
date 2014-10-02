package net.pic4pic.ginger.service;

/*package*/ class ServiceEndpoint{
	
	/*package*/ static final ServiceEndpoint MainService = new ServiceEndpoint(true, "svc.pic4pic.net");
	/*package*/ static final ServiceEndpoint ImageService = new ServiceEndpoint(false, "svc.pic4pic.net");
	/*package*/ static final ServiceEndpoint InstantMessageService = new ServiceEndpoint(true, "svc.pic4pic.net");
	/*package*/ static final ServiceEndpoint AnalyticService = new ServiceEndpoint(true, "svc.pic4pic.net");
	/*package*/ static final ServiceEndpoint LogService = new ServiceEndpoint(true, "svc.pic4pic.net");
	
	private boolean ssl;
	private String url;
	
	public ServiceEndpoint(boolean ssl, String url){
		
		this.ssl = ssl;
		this.url = url.trim();
	}
	
	public String getUrl(String path){
		
		// check input
		if(path != null){
			path = path.trim();	
		}
		else{
			path = "";
		}			 
		
		// http://
		String uri = this.ssl ? "https://" : "http://";
		
		// http://pic4pic-web-svc.cloudapp.net
		uri += this.url;
		
		// http://pic4pic-web-svc.cloudapp.net/
		if(!url.endsWith("/") && !path.startsWith("/")){
			uri += "/";
		}
		
		// http://pic4pic-web-svc.cloudapp.net/svc/rest/signin
		uri += path;
		
		// return
		return uri;
	}
}

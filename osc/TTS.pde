import java.net.*;
import java.io.*;

void textToSound(String text) {
 googleTTS(text, "en");
}


void googleTTS(String txt, String language) {
  String u = "http://translate.google.com/translate_tts?tl=";
  String fileName="";
  u = u + language + "&q=" + txt;
  u = u.replace(" ", "%20");
  try {
    URL url = new URL(u);
    try {
      URLConnection connection = url.openConnection();
      connection.setRequestProperty("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705; .NET CLR 1.1.4322; .NET CLR 1.2.30703)");
      connection.connect();
      InputStream is = connection.getInputStream();
      fileName=sketchPath + "/data/" + txt + ".mp3";
      File f = new File(fileName);

      OutputStream out = new FileOutputStream(f);
      byte buf[] = new byte[1024];
      int len;
      while ( (len = is.read (buf)) > 0) {
        out.write(buf, 0, len);
      }
      out.close();
      is.close();
      println("File created: " + txt + ".mp3");
    } 
    catch (IOException e) {
      e.printStackTrace();
    }
  } 
  catch (MalformedURLException e) {
    e.printStackTrace();
  }


}


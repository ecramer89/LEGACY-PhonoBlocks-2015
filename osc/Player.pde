import ddf.minim.spi.*;
import ddf.minim.signals.*;
import ddf.minim.*;
import ddf.minim.analysis.*;
import ddf.minim.ugens.*;
import ddf.minim.effects.*;
Minim m;
AudioPlayer player;

void play(String fileName){
  println(fileName);
  player=m.loadFile(fileName);
  player.rewind();
  
  player.play();
  
  
}

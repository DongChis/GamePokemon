public class Position
{
    public int posX;
    
   public int posY;
    public Position()
    {

    }
    public Position(int x, int y)
    {
        this.posX = x;
        this.posY = y;
    }

    public void setPosition(int x, int y)
    {
        this.posX = x; 
        this.posY = y;
    }
}
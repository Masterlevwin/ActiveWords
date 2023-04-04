using Random = UnityEngine.Random;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
  public int columns = 8;   // Number of Columns on our 2D board
  public int rows = 8;      // Number of Rows on our 2D board
  
  public GameObject[] gameTiles;    // Array of base Tile prefabs
  public GameObject[] wallTiles;    // Array of outer wall Tile prefabs
  public GameObject[] enemyTiles;   // Array of enemy prefabs
  public GameObject[] pickupTiles;  // Array of pick-up prefabs
  public GameObject exit;   // Object for our exit
  
  private Transform boardHolder;  // Use to child all our GameObjects to keep the hierarchy clean
  private List<Vector3> gridPositions = new List<Vector3>();  // List of valid locations to place tiles upon
  
  void InitialiseList()
  {
    gridPositions.Clear();                  // clear tiles from last generation
    for( int x = 1; x < columns - 1; x++ )  // create our 2D array of valid tile locations
    {
      for( int y = 1; y < rows - 1; y++ )
      { 
        gridPositions.Add( new Vector3( x, y, 0f ) );
      }
    }
  }
  
void BoardSetup() {
// Initialize our board
boardHolder = new GameObject("Board").transform; 
for (int x = -1; x < columns + 1; x++) { 
 for (int y = -1; y < rows + 1; y++) { 
 // choose a random floor tile and prepare to instantiate it
 GameObject o = gameTiles[Random.Range(0, gameTiles.Length)];
 // If at edges, choose from outer wall tiles instead
 if (x == -1 || x == columns || y == -1 || y == rows) { 
 o = wallTiles[Random.Range(0, wallTiles.Length)];
 } 
 // Instantiate the chosen tile, at current grid position
 GameObject instance = Instantiate(
 o,
 new Vector3(x, y, 0f),
 Quaternion.identity) as GameObject 
 // Set parent of our new instance object to boardHolder
 instance.transform.SetParent(boardHolder);
 }
}
}

Vector3 RandomPosition(){ 
// random index between 0 and total count of items in gridPositions
int randomIndex = Random.Range(0, gridPositions.Count); 
// random position selected from our gridPosition using randomIndex
Vector3 randomPosition = gridPositions[randomIndex] 
// remove the entry from gridPosition so it can't be re-used
gridPositions.RemoveAt(randomIndex); 
return randomPosition;
}

void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max){
// random amount to instantiate from given range 
int objectCount = Random.Range(min, max+1); 
// Place objects at random locations until object count limit
for (int i = 0; i < objectCount; i++ { 
 // use our helper function to get random position
 Vector3 randomPosition = RandomPosition(); 
 // Choose a random tile (pick-up, enemy, etc.) 
 GameObject tileChoice = 
 tileArray[Random.Range(0, tileArray.Length)]; 
 // Instantiate the chosen tile at the chosen location
 Instantiate(tileChoice, randomPosition, Quaternion.identity);
}
}

public void SetupScene(int level){
// Call our function to create our outer walls and floor.
BoardSetup();
// Re-create our list of valid gridPositions
InitialiseList();
// Fill a random number of pick-up tiles 
LayoutObjectAtRandom(pickupTiles, 1, 3)
// Determine number of enemies based on current "level"
// Fill a random number of creatures 
int enemies = (int)Mathf.Log(level, 2f);
LayoutObjectAtRandom(enemyTiles, enemies, enemies); 
// Put in our exit in top right 
Instantiate(
 exit,
 new Vector3 (columns -1, rows -1, 0f),
 Quaternion.identity
);
}



}

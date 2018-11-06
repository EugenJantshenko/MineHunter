using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum Complexity
{
    Easy = 1,
    Medium = 2,
    Hard = 3,
    Insane = 4
};
public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
};


public class GamefieldController : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject PlayerPrefab;
    public Complexity current;
    public Direction currentDirection;
    [SerializeField] private int _tilesXCount;
    [SerializeField] private int _tilesZCount;
    [SerializeField] private float _distanceBetweenTiles = 0.2f;
    [SerializeField] private TextMesh _tileText;
    private bool[,] _isMined;
    private GameObject[,] _tiles;
    public GameObject[,] Tiles { get; set; }
    private TextMesh[,] _text;
    private MeshRenderer[,] _textMesh;
    private GameObject[] TileText;
    public int currentI, currentJ;
    private GameObject Player;



    void Start()
    {
        CreateTiles();
        CreatePlayer();
        var a = Enum.GetValues(typeof(Direction)).Length;
        Debug.Log(a);
    }
    private void Update()
    {
        InputController();
    }
    private void InputController()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HideNumbers();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            RotateLeft();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            RotateRight();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveTorwards();
        }
    }
    private void CreateTiles()
    {

        _isMined = new bool[_tilesXCount, _tilesZCount];
        _tiles = new GameObject[_tilesXCount, _tilesZCount];
        _text = new TextMesh[_tilesXCount, _tilesZCount];
        _textMesh = new MeshRenderer[_tilesXCount, _tilesZCount];

        Vector3 currentPosition = Vector3.zero;
        for (int i = 0; i < _tilesXCount; i++)
        {
            for (int j = 0; j < _tilesZCount; j++)
            {
                _tiles[i, j] = Instantiate(TilePrefab, currentPosition, transform.rotation);
                currentPosition = new Vector3(currentPosition.x + TilePrefab.transform.localScale.x
                                                + _distanceBetweenTiles, currentPosition.y,
                                                currentPosition.z);
                _text[i, j] = _tiles[i, j].GetComponentInChildren<TextMesh>();
                //_textMesh[i, j] = _tiles[i, j].GetComponentInChildren<MeshRenderer>();
                _text[i, j].text = "0";
            }
            currentPosition = new Vector3(0, currentPosition.y, currentPosition.z + TilePrefab.transform.localScale.z + _distanceBetweenTiles);

        }
        TileText = GameObject.FindGameObjectsWithTag("Tile");
        MassiveConverter();
        Mining();
    }
    private void Mining()
    {
        int _mineCount = 0;
        switch (current)
        {
            case Complexity.Easy:
                _mineCount = _tiles.Length / 10;
                break;
            case Complexity.Medium:
                _mineCount = _tiles.Length / 8;
                break;
            case Complexity.Hard:
                _mineCount = _tiles.Length / 6;
                break;
            case Complexity.Insane:
                _mineCount = _tiles.Length / 4;
                break;
            default: break;
        }
        for (int i = 0; i < _mineCount;)
        {
            int x = UnityEngine.Random.Range(0, _tilesXCount - 1);
            int y = UnityEngine.Random.Range(0, _tilesZCount - 1);
            if (_isMined[x, y] != true)
            {
                _isMined[x, y] = true;
                i++;
            }
        }

        for (int i = 0; i < _tilesXCount; i++)
        {
            for (int j = 0; j < _tilesZCount; j++)
            {
                if (_isMined[i, j])
                {
                    _text[i, j].text = "M";
                    // Destroy(_tiles[i, j]);
                }
            }

        }
        FieldCounting();
    }
    private void FieldCounting()
    {
        int _fieldcount = 0;
        for (int i = 0; i < _tilesXCount; i++)
        {
            for (int j = 0; j < _tilesZCount; j++)
            {
                if (_isMined[i, j])
                    continue;
                //Угловые точки
                if (i == 0 && j == 0)
                {
                    _fieldcount = 0;
                    if (_isMined[i + 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    _text[i, j].text = _fieldcount.ToString();
                }
                if (i == 0 && j == _tilesZCount - 1)
                {
                    _fieldcount = 0;
                    if (_isMined[i + 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    _text[i, j].text = _fieldcount.ToString();
                }
                if (i == _tilesXCount - 1 && j == _tilesZCount - 1)
                {
                    _fieldcount = 0;
                    if (_isMined[i - 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i - 1, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    _text[i, j].text = _fieldcount.ToString();
                }
                if (i == _tilesXCount && j == 0)
                {
                    _fieldcount = 0;
                    if (_isMined[i - 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i - 1, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    _text[i, j].text = _fieldcount.ToString();

                }
                //Центральный массив
                if (i != 0 && i != _tilesXCount - 1 && j != 0 && j != _tilesZCount - 1)
                {
                    _fieldcount = 0;
                    if (_isMined[i - 1, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i - 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i - 1, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    _text[i, j].text = _fieldcount.ToString();
                }
                //Боковые грани
                if (i == 0 && j != 0 && j != _tilesZCount - 1)
                {
                    _fieldcount = 0;
                    if (_isMined[i, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    _text[i, j].text = _fieldcount.ToString();
                }
                if (i != 0 && i != _tilesXCount - 1 && j == _tilesZCount - 1)
                {
                    _fieldcount = 0;
                    if (_isMined[i - 1, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i - 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    _text[i, j].text = _fieldcount.ToString();
                }
                if (i == _tilesXCount - 1 && j != 0 && j != _tilesZCount - 1)
                {
                    _fieldcount = 0;
                    if (_isMined[i - 1, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i - 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i - 1, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j - 1] == true)
                    {
                        _fieldcount++;
                    }
                    _text[i, j].text = _fieldcount.ToString();
                }
                if (i != 0 && i != _tilesXCount - 1 && j == 0)
                {
                    _fieldcount = 0;
                    if (_isMined[i - 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i - 1, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j + 1] == true)
                    {
                        _fieldcount++;
                    }
                    if (_isMined[i + 1, j] == true)
                    {
                        _fieldcount++;
                    }
                    _text[i, j].text = _fieldcount.ToString();
                }
            }

        }
    }
    private void HideNumbers()
    {
        for (int i = 0; i < _tilesXCount; i++)
        {
            for (int j = 0; j < _tilesZCount; j++)
            {
                _textMesh[i, j].enabled = false;
            }

        }
        //for (int i = 0; i < _tilesXCount * _tilesZCount; i++)
        //{
        //    TileText[i].SetActive(false);
        //}
    }
    private void MassiveConverter()
    {
        for (int i = 0; i < _tilesXCount; i++)
        {
            for (int j = 0; j < _tilesZCount; j++)
            {
                _textMesh[i, j] = TileText[i * _tilesZCount + j].GetComponent<MeshRenderer>();
            }

        }
    }
    private void CreatePlayer()
    {
        int x = UnityEngine.Random.Range(0, _tilesXCount - 1);
        int z = UnityEngine.Random.Range(0, _tilesZCount - 1);
        if (_text[x, z].text == "0")
        {
            Player = Instantiate(PlayerPrefab, _tiles[x, z].transform.position, Quaternion.identity);
            Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 0.3f, Player.transform.position.z);
            currentI = x;
            currentJ = z;
            Camera.main.transform.parent = Player.transform;
        }
        else CreatePlayer();
        currentDirection = Direction.North;
    }

    private void RotateLeft()
    {
        Player.transform.Rotate(0, -90, 0);
        if (currentDirection > Direction.North)
        {
            currentDirection--;
            Debug.Log(currentDirection);
        }
        else { currentDirection = Direction.West; Debug.Log(currentDirection); }
    }
    private void RotateRight()
    {
        Player.transform.Rotate(0, 90, 0);
        if (currentDirection < Direction.West)
        {
            currentDirection++;
            Debug.Log(currentDirection);
        }
        else { currentDirection = Direction.North; Debug.Log(currentDirection); }
    }
    private void MoveTorwards()
    {
        switch (currentDirection)
        {
            case Direction.North:
                {
                    Player.transform.position = _tiles[currentI+1, currentJ].transform.position;
                    currentI++;
                    break;
                }
            case Direction.East:
                {
                    Player.transform.position = _tiles[currentI, currentJ + 1].transform.position;
                    currentJ++;
                    break;
                }
            case Direction.South:
                {
                    Player.transform.position = _tiles[currentI - 1, currentJ].transform.position;
                    currentI--;
                    break;
                }
            case Direction.West:
                {
                    Player.transform.position = _tiles[currentI, currentJ - 1].transform.position;
                    currentJ--;
                    break;
                }
        }
    }

}

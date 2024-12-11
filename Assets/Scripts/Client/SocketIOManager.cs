using UnityEngine;
using SocketIOClient;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using SocketIOClient.Newtonsoft.Json;

public class SocketIOManager : Singleton<SocketIOManager>
{
    string filePath;
    private void Start()
    {
        DontDestroyOnLoad(this);
        filePath = Path.Combine(Application.dataPath, "../log.txt");
    }
    private SocketIOUnity client;

    private string serverUri = "ws://103.211.206.26:8080";

    public void StartConnection(string accessToken)
    {
        // Khởi tạo client Socket.IO
        client = new SocketIOUnity(new Uri(serverUri), new SocketIOOptions
        {
            Query = new Dictionary<string, string>
        {
            {"token", accessToken }
        }
    ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        client.JsonSerializer = new NewtonsoftJsonSerializer();
        // Kết nối
        client.OnConnected += (sender, e) =>
        {
            StartListening();
            // Lắng nghe sự kiện ERROR
            client.On("CLIENT_ERROR", (data) =>
            {
                Debug.Log("Error: " + data);
                ProcessError(data);
            });

        };
        //Huy ket noi
        client.OnDisconnected += (sender, e) =>
        {
            Debug.Log("Disconnected from server. Reason: " + e);
            File.AppendAllText(filePath, "Disconnect");
        };


        // Kết nối đến máy chủ
        client.ConnectAsync();
    }


    public void StartListening()
    {

        client.On("CLIENT_ROOM_INFORMATION", (data) =>
        {
            Debug.Log("Room Information: " + data);
            ProcessRoomData(data);
        });

        // Lắng nghe sự kiện PLAYER_JOIN_ROOM
        client.On("CLIENT_PLAYER_JOIN_ROOM", (data) =>
        {
            Debug.Log("Player Join Room: " + data);
            ProcessPlayerJoinRoomData(data);
        });
        client.On("CLIENT_PLAYER_READY", (data) =>
        {
            Debug.Log("Player ready: " + data);
            ProcessPlayerReadyData(data);
        });
        client.On("CLIENT_PLAYER_CANCEL_READY", (data) =>
        {
            Debug.Log("Player cancel ready: " + data);
            ProcessPlayerReadyData(data);
        });
        client.On("CLIENT_START_GAME", (data) =>
        {
            Debug.Log("Start Game: " + data);
            ProcessStartGameData(data);
        });

        client.On("CLIENT_GET_VALID_MOVES", (data) =>
        {
            Debug.Log("Return Move: " + data);

            File.AppendAllText(filePath, data.ToString());
            ProcessValidMoveData(data);
        });
        client.On("CLIENT_MOVE_PIECE", (data) =>
        {
            Debug.Log("Return Move: " + data);
            ProcessMoveInData(data);
        });
        client.On("CLIENT_KING_IN_CHECK", (data) =>
        {
            Debug.Log("Return Move: " + data);
            ProcessCheckMate(data);
        });

        client.On("CLIENT_KING_IN_CHECK", (data) =>
        {
            Debug.Log("Checkmate: " + data);
            ProcessCheckMate(data);
        });

        client.On("CLIENT_GAME_OVER", (data) =>
        {
            Debug.Log("GameOver: " + data);
            ProcessGameOver(data);
        });
    }

    public void SendReady(bool isReady)
    {
        if (isReady)
        {
            client.EmitAsync("SERVER_PLAYER_READY");
            Debug.Log("Event SERVER_PLAYER_READY sent.");
        }
        else
        {
            client.EmitAsync("SERVER_PLAYER_CANCEL_READY");
            Debug.Log("Event CLIENT_PLAYER_CANCEL_READY sent.");
        }
    }

    public void SendChessMen(Vector2 v)
    {
        string gameId = GameManager.Instance.gameRoom.gameId;

        var chessData = new Dictionary<string, object>
        {
            { "x", v.x },
            { "y", v.y },
            { "gameId", gameId }
        };

        // Emit the data as a JSON string
        client.Emit("SERVER_GET_VALID_MOVES", chessData);
    }
    public void SendMoveIn(Vector2 v)
    {
        string gameId = GameManager.Instance.gameRoom.gameId;
        Vector2 startPos = ChessMen.choosenChessMen.curCoordnates;
        if (!GameManager.Instance.isRed())
        {
            startPos.x = 9 - startPos.x;
            v.x = 9 - v.x;
        }
        var moveData = new Dictionary<string, object>
        {
            {"fromX", startPos.x },
            {"fromY", startPos.y },
            { "toX", v.x },
            { "toY", v.y },
        };
        File.AppendAllText(filePath, "BUG");
        // Emit the event SERVER_GET_VALID_MOVES with the move data
        client.EmitAsync("SERVER_MOVE_PIECE", moveData);
    }
    private void ProcessPlayerJoinRoomData(SocketIOResponse jsonData)
    {
        try
        {
            string jsonResponse = jsonData.ToString();
            Debug.Log("Chuỗi JSON: " + jsonResponse);

            var userProfiles = JsonUtility.FromJson<UserProfileDataWrapper>("{\"userProfiles\":" + jsonResponse + "}");

            // Đưa mọi thao tác UI vào hàng đợi để thực hiện trên luồng chính
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                foreach (var userProfile in userProfiles.userProfiles)
                {
                    GameManager.Instance.RoomData.userProfiles.Add(userProfile);
                }
                GamePlayUIScript.Instance.Refresh(); // Gọi Refresh để cập nhật UI
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi khi xử lý dữ liệu tham gia phòng: " + ex.Message);
        }
    }
    private void ProcessPlayerReadyData(SocketIOResponse data)
    {
        try
        {
            string jsonResponse = data.ToString();
            Debug.Log("Chuỗi JSON: " + jsonResponse);

            var userProfiles = JsonUtility.FromJson<UserProfileDataWrapper>("{\"userProfiles\":" + jsonResponse + "}");

            // Đưa mọi thao tác UI vào hàng đợi để thực hiện trên luồng chính
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                foreach (var userProfile in userProfiles.userProfiles)
                {
                    for (int i = 0; i < GameManager.Instance.RoomData.userProfiles.Count; i++)
                    {
                        if (GameManager.Instance.RoomData.userProfiles[i].id == userProfile.id)
                        {
                            GameManager.Instance.RoomData.userProfiles[i] = userProfile;
                        }
                    }
                }
                GamePlayUIScript.Instance.Refresh(); // Gọi Refresh để cập nhật UI
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi khi xử lý dữ liệu tham gia phòng: " + ex.Message);
        }
    }
    private void ProcessStartGameData(SocketIOResponse data)
    {
        try
        {
            var jsonObject = JsonConvert.DeserializeObject<JObject>("{\"gameRooms\":" + data.ToString() + "}");

            // Now deserialize the gameRooms array to GameRoomWrapper
            var wrapper = jsonObject.ToObject<GameRoomWrapper>();

            // Access the first item in the array
            GameRoom gameRoom = wrapper.gameRooms[0];

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                // Check if playerIdToColorMap exists and has entries
                if (gameRoom.playerIdToColorMap == null)
                {
                    Debug.LogError("playerIdToColorMap is null after deserialization.");
                }
                else if (gameRoom.playerIdToColorMap.Count == 0)
                {
                    Debug.LogError("playerIdToColorMap has no entries after deserialization.");
                }
                else
                {
                    // Log entries in the playerIdToColorMap
                    foreach (var entry in gameRoom.playerIdToColorMap)
                    {
                        Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");
                    }
                }

                // Set the game room instance
                GameManager.Instance.gameRoom = gameRoom;

                // Generate the board and start the game
                BoardGenerator.Instance.Generate(gameRoom.board);
                GamePlayUIScript.Instance.StartGame(); // Gọi Refresh để cập nhật UI
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi khi xử lý dữ liệu tham gia phòng: " + ex.Message);
        }
    }
    private void ProcessRoomData(SocketIOResponse jsonData)
    {
        // Lấy phản hồi JSON thô dưới dạng chuỗi
        string jsonResponse = jsonData.ToString();

        if (string.IsNullOrEmpty(jsonResponse))
        {
            Debug.LogWarning("Raw JSON Response is null or empty.");
            return;
        }

        // Sử dụng main thread của Unity để xử lý dữ liệu
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                // Deserialize phản hồi JSON thành dữ liệu phòng
                ListRoomData roomData = JsonUtility.FromJson<ListRoomData>("{\"rooms\":" + jsonResponse + "}");

                // Cập nhật GameManager với dữ liệu phòng đầu tiên
                if (roomData.rooms.Length > 0)
                {
                    GameManager.Instance.RoomData = roomData.rooms[0];
                    GamePlayUIScript.Instance.Refresh();
                    Debug.Log("Room data saved, updating UI.");
                }
                else
                {
                    Debug.LogWarning("No rooms found in the response.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error retrieving JSON data: " + ex.Message);
            }
        });
    }
    private void ProcessValidMoveData(SocketIOResponse data)
    {
        File.AppendAllText(filePath, data.ToString());
        try
        {
            // Get the raw JSON response as a string
            string jsonResponse = data.ToString();

            if (string.IsNullOrEmpty(jsonResponse))
            {
                Debug.LogWarning("Raw JSON Response is null or empty.");
                return;
            }

            // Deserialize the JSON response into a list of ValidMoveData objects
            List<ValidMoveData> validMoveDataList = JsonConvert.DeserializeObject<List<ValidMoveData>>(jsonResponse);

            // Enqueue the processing of valid move data to the main thread
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                // Process each ValidMoveData object
                foreach (var validMoveData in validMoveDataList)
                {
                    foreach (var point in validMoveData.points)
                    {
                        Debug.Log($"Point: x = {point.x}, y = {point.y}");
                        if (GameManager.Instance.isRed())
                        {
                            // Update the board state for the valid moves
                            BoardGenerator.Instance.points[new Vector2(point.x, point.y)].SetCanMove(ChessMen.choosenChessMen);
                        }else BoardGenerator.Instance.points[new Vector2(9 - point.x, point.y)].SetCanMove(ChessMen.choosenChessMen);

                    }
                }
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("Error retrieving JSON data: " + ex.Message);
        }
    }
    private void ProcessMoveInData(SocketIOResponse data)
    {
        try
        {
            // Get the raw JSON response as a string
            string jsonResponse = data.ToString();

            if (string.IsNullOrEmpty(jsonResponse))
            {
                Debug.LogWarning("Raw JSON Response is null or empty.");
                return;
            }

            // Deserialize the JSON response into a list of MoveData objects
            List<MoveData> moveDataList = JsonConvert.DeserializeObject<List<MoveData>>(jsonResponse);

            var move = moveDataList[0];
            if (!GameManager.Instance.isRed())
            {
                Debug.Log(1);
                move.fromX = 9 - move.fromX;
                move.toX = 9 - move.toX;
            }
            Debug.Log($"Move from ({move.fromX}, {move.fromY}) to ({move.toX}, {move.toY})");
            // Enqueue the processing of valid move data to the main thread

            File.AppendAllText(filePath, "Move To " + move.toX.ToString() + ", " + move.toY.ToString());
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                BoardGenerator.Instance.points[new Vector2(move.toX, move.toY)].Movein(
                    BoardGenerator.Instance.points[new Vector2(move.fromX, move.fromY)].Chess);
            });
        }
        catch (Exception ex)
        {
            File.AppendAllText(filePath, "Error retrieving JSON data: " + ex.Message);
            Debug.LogError("Error retrieving JSON data: " + ex.Message);
        }
    }

    private void ProcessCheckMate(SocketIOResponse data)
    {
        try
        {
            // Get the raw JSON response as a string
            string jsonResponse = data.ToString();

            if (string.IsNullOrEmpty(jsonResponse))
            {
                Debug.LogWarning("Raw JSON Response is null or empty.");
                return;
            }

            // Deserialize the JSON response into a CheckMateData object
            List<CheckMateData> checkMateData = JsonConvert.DeserializeObject<List<CheckMateData>>(jsonResponse);
            string color = checkMateData[0].piece.color;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                CheckMateUIScript.Instance.StartAnim(color);
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("Error retrieving JSON data: " + ex.Message);
        }
    }

    private void ProcessGameOver(SocketIOResponse data)
    {
        try
        {
            // Get the raw JSON response as a string
            string jsonResponse = data.ToString();
            Debug.Log(jsonResponse);

            if (string.IsNullOrEmpty(jsonResponse))
            {
                Debug.LogWarning("Raw JSON Response is null or empty.");
                return;
            }

            // Deserialize the JSON response into a GameResult object
            List<GameResult> gameResults = JsonConvert.DeserializeObject<List<GameResult>>(jsonResponse);

            if (gameResults != null && gameResults.Count > 0)
            {
                GameResult result = gameResults[0]; // Get the first result

                // You can now access winner and loser information
                Debug.Log($"Winner: {result.winner.userProfile.fullName}, Loser: {result.loser.userProfile.fullName}");

                // Example: Start the end game animation or UI
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    EndGameUIScript.Instance.Init(result.winner.id == GameManager.Instance.user.userId);
                });
            }
            else
            {
                Debug.LogWarning("No game result data found after deserialization.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error retrieving JSON data: " + ex.Message);
        }
    }
    private void ProcessError(SocketIOResponse data)
    {
        try
        {
            List<MessageData> messageDataList = JsonConvert.DeserializeObject<List<MessageData>>(data.ToString());

            if (messageDataList != null && messageDataList.Count > 0)
            {
                string message = messageDataList[0].ProcessMessage(); // Lấy giá trị message
                Debug.Log(message); // In ra giá trị

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    NotifyUIScript.Instance.StartNotify(message);
                });
            }
            else
            {
                Debug.LogWarning("No message data found.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error retrieving JSON data: " + ex.Message);
        }
    }
    public void CreateRoom()
    {
        // Gửi sự kiện SERVER_CREATE_ROOM mà không có dữ liệu
        client.EmitAsync("SERVER_CREATE_ROOM");
        Debug.Log("Event SERVER_CREATE_ROOM sent.");
    }
    public void LeaveRoom()
    {
        // Gửi sự kiện SERVER_CREATE_ROOM mà không có dữ liệu
        client.EmitAsync("SERVER_LEAVE_ROOM");
        Debug.Log("Event SERVER_LEAVE_ROOM sent.");
    }
    public void JoinRoom(string roomId)
    {
        // Tạo đối tượng dữ liệu để gửi
        var room = new Dictionary<string, string>
        {
            { "roomId", roomId }
        };

        // Gửi sự kiện SERVER_JOIN_ROOM với thông điệp roomId
        client.EmitAsync("SERVER_JOIN_ROOM", room);
        Debug.Log("Event SERVER_JOIN_ROOM sent with roomId: " + roomId);
        SceneManager.LoadScene("GamePlay");

    }

    private void OnDestroy()
    {
        // Ngắt kết nối khi đối tượng bị hủy
        client.DisconnectAsync();
    }
}
[Serializable]
public class MessageData
{
    public string message { get; set; }
    public string ProcessMessage()
    {
        // Thay thế dấu gạch ngang bằng dấu cách và chuyển thành chữ hoa
        return message.Replace("-", " ").ToUpper();
    }
}

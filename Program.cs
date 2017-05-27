using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.OleDb;
using System.Diagnostics;

namespace TicTacToe
{


    class Program
    {
        static int maxdepth = 9;



        static void Main(string[] args)
        {
            Boolean continuePlaying = false;
            char[] ttt_board = new char[9];


            
            var stopwatch = new Stopwatch();
            String winner;
            continuePlaying = continueGame(continuePlaying);

            while (continuePlaying)
            {
                resetBoard(ttt_board);

                stopwatch.Start();
                winner = gameLoop(ttt_board);
                stopwatch.Stop();
                Console.WriteLine("Winner: " + winner);
                Console.WriteLine("Milliseconds: " + stopwatch.ElapsedMilliseconds);
                insertGameStats((int)stopwatch.ElapsedMilliseconds, winner);

                continuePlaying = continueGame(continuePlaying);
            }
            retrieveData();
            
            Console.ReadKey();

        }

        static String gameLoop(char[] ttt_board)
        {
            String winner = "Null";
            Random rnd = new Random();
            int firstPlayer = rnd.Next(2);
            if (firstPlayer % 2 == 1)
            {
                computerTurn(ttt_board);
                Console.WriteLine("Computer went first");
            }


            while (winner == "Null")
            {
                printBoard(ttt_board);

                playerTurn(ttt_board);
                winner = gameStateEvaluation(ttt_board);
                if (winner == "Null")
                {
                    computerTurn(ttt_board);
                }

                winner = gameStateEvaluation(ttt_board);
            }
            printBoard(ttt_board);
            return winner;
        }

        static String gameStateEvaluation(char[] ttt_board)
        {

            String winner = "Null";

            if (checkForWinner(ttt_board, 'P'))
            {
                winner = "Player";
            }
            else if (checkForWinner(ttt_board, 'C'))
            {
                winner = "Computer";

            }
            else if (checkForStall(ttt_board))
            {
                winner = "Stalemate";
            }


            return winner;
        }

        static Boolean checkForStall(char[] ttt_board)
        {
            Boolean stall = true;
            for (int i = 0; i < 9; i++)
            {

                if (ttt_board[i] != 'P' && ttt_board[i] != 'C')
                {
                    stall = false;
                }
            }

            return stall;
        }

        static Boolean checkForWinner(char[] ttt_board, char player)
        {
            Boolean won = false;

            if (ttt_board[0] == ttt_board[3] && ttt_board[3] == ttt_board[6] && ttt_board[6] == player)
            {
                won = true;
            }
            if (ttt_board[1] == ttt_board[4] && ttt_board[4] == ttt_board[7] && ttt_board[7] == player)
            {
                won = true;
            }
            if (ttt_board[2] == ttt_board[5] && ttt_board[5] == ttt_board[8] && ttt_board[8] == player)
            {
                won = true;
            }
            if (ttt_board[0] == ttt_board[1] && ttt_board[1] == ttt_board[2] && ttt_board[2] == player)
            {
                won = true;
            }
            if (ttt_board[3] == ttt_board[4] && ttt_board[4] == ttt_board[5] && ttt_board[5] == player)
            {
                won = true;
            }
            if (ttt_board[6] == ttt_board[7] && ttt_board[7] == ttt_board[8] && ttt_board[8] == player)
            {
                won = true;
            }
            if (ttt_board[0] == ttt_board[4] && ttt_board[4] == ttt_board[8] && ttt_board[8] == player)
            {
                won = true;
            }
            if (ttt_board[2] == ttt_board[4] && ttt_board[4] == ttt_board[6] && ttt_board[6] == player)
            {
                won = true;
            }

            return won;
        }


        static void playerTurn(char[] ttt_board)
        {
            int playerMove = -1;
            while (playerMove < 0 || playerMove > 9)
            {
                Console.Write("Please enter your move[0-8]: ");
                playerMove = Convert.ToInt16(Console.ReadLine());

                if (playerMove < 0 || playerMove > 8) { Console.WriteLine("Invalid Command"); }
                else
                {

                    if (invalidMoveChecker(ttt_board, playerMove))
                    {
                        playerMove = -1;
                        Console.WriteLine("Invalid Move, place already taken");
                    }
                }
            }
            ttt_board[playerMove] = 'P';
        }

        static void computerTurn(char[] ttt_board)
        {
            int best = -20000, depth = 0, score, computerMove = 0;

            for (int i = 0; i < 9; i++)
            {
                if (!invalidMoveChecker(ttt_board, i))
                {
                    ttt_board[i] = 'C';
                    score = min(ttt_board, depth + 1);
                    if (score > best)
                    {
                        computerMove = i;
                        best = score;
                    }
                    ttt_board[i] = Convert.ToChar(i + 48);
                }
            }


            Console.WriteLine("Computer Move: " + computerMove);
            ttt_board[computerMove] = 'C';

        }

        static int min(char[] ttt_board, int depth)
        {
            int best = 2000, score;
            String gameState = gameStateEvaluation(ttt_board);
            if (gameState != "Null")
            {
                if (gameState == "Player")
                    return -5000;
                else if (gameState == "Computer")
                    return 5000;
                else
                    return 0;
            }

            if (depth == maxdepth)
                return 0;

            for (int i = 0; i < 9; i++)
            {
                if (!invalidMoveChecker(ttt_board, i))
                {
                    ttt_board[i] = 'P';
                    score = max(ttt_board, depth + 1);
                    if (score < best)
                        best = score;
                    ttt_board[i] = Convert.ToChar(i + 48); // Undo move
                }
            }

            return best;
        }

        static int max(char[] ttt_board, int depth)
        {
            int best = -20000, score;
            String gameState = gameStateEvaluation(ttt_board);
            if (gameState != "Null")
            {
                if (gameState == "Player")
                    return -5000;
                else if (gameState == "Computer")
                    return 5000;
                else
                    return 0;
            }

            if (depth == maxdepth)
                return 0;

            for (int i = 0; i < 9; i++)
            {
                if (!invalidMoveChecker(ttt_board, i))
                {
                    ttt_board[i] = 'C';
                    score = min(ttt_board, depth + 1);
                    if (score > best)
                        best = score;
                    ttt_board[i] = Convert.ToChar(i + 48); // Undo move
                }
            }


            return best;
        }


        static Boolean invalidMoveChecker(char[] ttt_board, int move)
        {
            Boolean invalidMove = false;
            if (ttt_board[move] == 'P' || ttt_board[move] == 'C')
                invalidMove = true;

            return invalidMove;
        }

        static void resetBoard(char[] ttt_board)
        {
            for (int i = 0; i < 9; i++)
            {
                ttt_board[i] = Convert.ToChar(i + 48);
            }
        }

        static void printBoard(char[] ttt_board)
        {
            Console.WriteLine("");
            for (int i = 0; i < 9; i++)
            {
                Console.Write(" " + ttt_board[i]);

                if ((i + 1) % 3 == 0)
                {
                    Console.WriteLine();
                }
            }
        }
        static Boolean continueGame(Boolean continuePlaying)
        {
            continuePlaying = false;
            Console.Write("Care to play a Game? (y/n): ");
            if (Console.ReadLine() == "y")
            {
                continuePlaying = true;
            }
            return continuePlaying;
        }

        static void insertGameStats(int time, String winner)
        {
            System.Data.OleDb.OleDbConnection conn = new
                System.Data.OleDb.OleDbConnection();
            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                @"Data source= C:\Users\Chris\Documents\CSC\Visual Studio\TicTacToe\TicTacToe\" +
                @"tictactoe.mdb";
            try
            {
                conn.Open();
                String insertString = $"insert into gameHistory ([ms], [winner]) values ({time}, '{winner}')";
                OleDbCommand cmd = new OleDbCommand(insertString, conn);
                cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to Insert Data");
            }
            finally
            {
                conn.Close();
            }
        }

        static void retrieveData()
        {
            System.Data.OleDb.OleDbConnection conn = new
                System.Data.OleDb.OleDbConnection();
            // TODO: Modify the connection string and include any
            // additional required properties for your database.
            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                @"Data source= C:\Users\Chris\Documents\CSC\Visual Studio\TicTacToe\TicTacToe\" +
                @"tictactoe.mdb";

            //@"Data source= C:\Users\Chris\Documents"
            try
            {
                conn.Open();

                OleDbCommand cmd = new OleDbCommand("Select top 8 * FROM gameHistory order by ID desc", conn);
                OleDbDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("Game History");
                Console.WriteLine("\t{0,-10} {1,6} {2,10}\n", "Game ID", "Milliseconds", "Winner");

                while (reader.Read())
                {
                    Console.WriteLine("\t  {0,-11} {1,-13} {2,-1}\n", reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), reader.GetValue(2).ToString());
                }

                // Insert code to process data.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to retrieve data from DB");
            }
            finally
            {
                conn.Close();
            }
        }

    }
}

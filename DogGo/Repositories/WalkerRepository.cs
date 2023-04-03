using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkerRepository : IWalkerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkerRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walker> GetAllWalkers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id, w.[Name], w.ImageUrl, nh.[Name] AS NeighborhoodName FROM Walker w
                            LEFT JOIN Neighborhood nh ON nh.Id = w.NeighborhoodId;
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Walker> walkers = new List<Walker>();
                        while (reader.Read())
                        {
                            Walker walker = new Walker
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                                Neighborhood = new Neighborhood()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                                }
                            };
                            walkers.Add(walker);
                        }

                        return walkers;
                    }
                }
            }
        }

        public List<Walker> GetWalkersInNeighborhood(int neighborhoodId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id, w.[Name], w.ImageUrl, n.[Name] AS NeighborhoodName FROM Walker w
                                        LEFT JOIN Neighborhood n ON n.Id = w.NeighborhoodId
                                        WHERE NeighborhoodId = @neighborhoodId";

                    cmd.Parameters.AddWithValue("@neighborhoodId", neighborhoodId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        List<Walker> walkers = new List<Walker>();
                        while (reader.Read())
                        {
                            Walker walker = new Walker
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                                Neighborhood = new Neighborhood()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                                }
                            };

                            walkers.Add(walker);
                        }

                        return walkers;
                    }
                }
            }
        }

        public Walker GetWalkerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id, w.[Name], w.ImageUrl, nh.[Name] AS NeighborhoodName FROM Walker w
                            LEFT JOIN Neighborhood nh ON nh.Id = w.NeighborhoodId
                        WHERE w.Id = @id;
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Walker walker = new Walker
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                                Neighborhood = new Neighborhood()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                                }
                            };

                            return walker;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public List<Walk> GetRecentWalksByWalker(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT a.Id, a.[Date], a.Duration,
	                        d.[Name] AS OwnerName
                        FROM Walks a
	                        LEFT JOIN Walker b ON b.Id = a.WalkerId
	                        LEFT JOIN Dog c ON c.Id = a.DogId
	                        LEFT JOIN [Owner] d ON d.Id = c.OwnerId
                        WHERE a.Id = @id";
                    // if this were truly going to be 'recent,' could add a WHERE for Date...

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Walk> walks = new List<Walk>();
                        while (reader.Read())
                        {
                            Walk walk = new Walk()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                // convert Duration to minutes from seconds
                                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                Owner = new Owner()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("OwnerName"))
                                }
                            };
                            walks.Add(walk);
                        }

                        return walks;
                    }
                }
            }
        }
    }
}
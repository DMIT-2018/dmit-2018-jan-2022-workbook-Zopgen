#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region additional namespaces
using ChinookSystem.DAL;
using ChinookSystem.Entities;
using ChinookSystem.ViewModels;
#endregion


namespace ChinookSystem.BLL
{
    public class PlaylistTrackServices
    {
        #region Constructor and Context Dependency
        private readonly ChinookContext _context;
        //obtain the context link from IServiceCollection when
        // this set of services is injected into the "outside user"
        internal PlaylistTrackServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        public List<PlaylistTrackInfo> PlaylistTrack_GetUserPlaylistTracks(string playlistname,
                                                                            string username)
        {
            IEnumerable<PlaylistTrackInfo> info = _context.PlaylistTracks
                                                    .Where(x => x.Playlist.Name.Equals(playlistname)
                                                            && x.Playlist.UserName.Equals(username))
                                                    .Select(x => new PlaylistTrackInfo
                                                    {
                                                        TrackId = x.TrackId,
                                                        TrackNumber = x.TrackNumber,
                                                        SongName = x.Track.Name,
                                                        Milliseconds = x.Track.Milliseconds
                                                    })
                                                    .OrderBy(x => x.TrackNumber);
            return info.ToList();
        }
        #endregion

        #region commands
        public void PlaylistTrack_AddTrack(string playlistname, string username, int trackid)
        {
            //create local variables
            Track trackExist = null;
            Playlist playlistExists = null;
            PlaylistTrack playlisttrackExists = null;
            int tracknumber = 0;

            //create a List<Exception> to contain all discovered errors
            List<Exception>errorlist = new List<Exception>();

            //buisness logic
            //these are procesing rules that need to be satiosfied for valid data
            //rule: a track can only exists once on a playlist
            //rule: each track on a playlist is assigned a continuous track number

            //if the rules are passed, consider that data valid, then   
            //a) stage your transactin wqork (Adds, Updates, Delete)
            //b) execute a single .SaveChanges() - commits toi database

            //parameter validatin
            if(string.IsNullOrWhiteSpace(playlistname))
            {
                throw new ArgumentNullException("Playlist name is missing");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("user name is missing");
            }

            trackExist = _context.Tracks
                        .Where(x => x.TrackId == trackid)
                        .FirstOrDefault();
            if (trackExist == null)
            {
                errorlist.Add(new Exception("Selected track no longer is on file. Refresh track table."));
            }

            //buisnbess process
            playlistExists = _context.Playlists
                            .Where(x => x.Name.Equals(playlistname)
                                    && x.UserName.Equals(username))
                            .FirstOrDefault();

            if (playlistExists == null)
            {
                //new playlist
                playlistExists = new Playlist()
                {
                    Name = playlistname,
                    UserName = username
                };
                //stage (only in memory)
                _context.Playlists.Add(playlistExists);
                tracknumber = 1;
            }
            else
            {
                //playlist already exists
                //rule: unique tracks on playlist
                playlisttrackExists = _context.PlaylistTracks
                                        .Where(x => x.Playlist.Name.Equals(playlistname)
                                                && x.Playlist.UserName.Equals(username)
                                                && x.TrackId == trackid)
                                        .FirstOrDefault();
                if(playlisttrackExists != null)
                {
                    var songname = _context.Tracks
                                    .Where(x => x.TrackId == trackid)
                                    .Select(x => x.Name)
                                    .SingleOrDefault();
                    errorlist.Add(new Exception($"Selected track ({songname})is already on the playlist"));
                }
                else
                {
                    tracknumber = _context.PlaylistTracks
                        .Where(x => x.Playlist.Name.Equals(playlistname)
                                && x.Playlist.UserName.Equals(username))
                        .Count();
                    tracknumber++;
                }
            }
            //add the track to the playlist
            //create an instance for the playlist track
            playlisttrackExists = new PlaylistTrack();

            //load values
            playlisttrackExists.TrackId = trackid;
            playlisttrackExists.TrackNumber =- tracknumber;

            //?? what about the second part of the primary key: PlayListID?
            // if the playlist exists then we klnow the playlist id: playlistExists.PlaylistID
            //BUT if the playlist is NEW, we DO NOT know the id

            // int the situation of anew playlist even tho we have created the playlistr instance 
            // it is only staged
            //this means the actual sql record has not been breated
            // this means the identity value for the playlist DOES NOT yet exist.
            //the value oin the playlist instance (playlistExists)
            //  is 0

            //Solution
            //it is built into the entity framework software and is based using the
            //  navigational property in playlits pointing to its child

            playlistExists.PlaylistTracks.Add(playlisttrackExists);

            //Staging is complete
            //Commit the work (Transaction)
            //commiting the work....tbd

            if (errorlist.Count > 0)
            {
                //thro the list oif buisness processing error(s)
                throw new AggregateException("Unable to add new track. Check concerns", errorlist);
            }
            else
            {
                //consider data valid
                _context.SaveChanges();
            }
        }
        #endregion
    }
}

select b.*
from TelloCommandObservation a
join AirSpeedObservation b on b.SessionId=a.SessionId and b.Timestamp BETWEEN a.Initiated and a.Completed 
where a.Id='fc6c3259-6bdb-429f-a5a4-400f1af039cc'
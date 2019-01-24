import route
import state

status = {
    'running': False,
    'waypoints': [],
    'route': [],
}

def step():
    
    if not status['running']:
        
        # check when to start running
        if 'autopilot_waypoints' in state.status and state.status['autopilot_waypoints'] != status['waypoints'] and 'solarsystemid2' in state.status:
            
            status['waypoints'] = list(state.status['autopilot_waypoints'])
            
            ssfrom = state.status['solarsystemid2']
            status['route'] = []
            
            # route waypoints
            for ssto in status['waypoints']:
                rt = route.dijsktra(route.graph, ssfrom, ssto)
                
                if len(rt) == 0:
                    break
                
                if len(status['route']) == 0:
                    status['route'] = rt
                else:
                    status['route'].extend(rt[1:])
    
    
    else:
        print(1)

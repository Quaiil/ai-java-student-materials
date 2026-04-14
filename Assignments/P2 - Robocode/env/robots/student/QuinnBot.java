package student;

import robocode.*;
import robocode.util.Utils;

public class QuinnBot extends AdvancedRobot {
    enum state{
        NONE,
        LOOKING,
        LOCKED,
    }
    
    state botState = state.NONE;

    public void run() {

        switch(botState)
        {
            case NONE:
                botState = state.LOOKING;
                break;
            case LOOKING:
                look();
                break;
            case LOCKED:
                break;
            default:
                botState = state.NONE;
        }
        
    }

    public void onScannedRobot(ScannedRobotEvent e) {
        botState = state.LOCKED;
        double angleToEnemy = getHeadingRadians() + e.getBearingRadians();
        double radarTurn = Utils.normalRelativeAngle(angleToEnemy - getRadarHeadingRadians());
        double botTurn = Utils.normalRelativeAngle(angleToEnemy - getHeadingRadians());
        double gunTurn = Utils.normalRelativeAngle(angleToEnemy - getGunHeadingRadians());
        double extraTurn = Math.min(Math.atan(36.0 / e.getDistance()), Rules.RADAR_TURN_RATE_RADIANS);

        radarTurn += (radarTurn < 0 ? -extraTurn : extraTurn);

        setTurnRadarRightRadians(radarTurn);
        setTurnGunRightRadians(gunTurn + 0.5);
        
        if (e.getDistance() < 200)
        {
            fire(3);
        }
        else
        {
            //setTurnRightRadians(botTurn);
            //ahead(400);
            fire(1);
        }
    }

    public void look()
    {
        if(getEnergy() > 50)
        {
            setTurnRadarRightRadians(Double.POSITIVE_INFINITY);
            setTurnGunRightRadians(Double.POSITIVE_INFINITY);
        }
    }
}

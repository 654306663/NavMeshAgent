using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject targetGo;

    public List<NavMeshAgent> agents;

    void Start()
    {
        StartCoroutine( IUpdate() );
    }


    IEnumerator IUpdate()
    {
        while ( true )
        {

            // 按下鼠标，开始寻路
            if ( Input.GetMouseButtonDown( 0 ) )
            {
                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
                RaycastHit hitInfo;
                if ( Physics.Raycast( ray, out hitInfo ) )
                {
                    targetGo.transform.position = hitInfo.point + new Vector3(0, 0.5f, 0);

                    for ( int i = 0; i < agents.Count; i++ )
                    {
                        agents[i].GetComponent<NavMeshObstacle>().enabled = false;
                        yield return null;      // 这里必须要等待一帧，否则 下一次寻路就会错位。
                        agents[i].enabled = true;
                        agents[i].SetDestination( hitInfo.point );
                    }
                }
            }

            yield return null;

            // 动态设置优先级
            if ( agents[0].enabled )
            {
                agents.Sort( ( x, y ) =>
                {
                    if ( x.enabled && y.enabled )
                    {
                        return x.remainingDistance.CompareTo( y.remainingDistance );
                    }
                    return 0;
                } );

                for ( int i = 0; i < agents.Count; i++ )
                {
                    agents[i].avoidancePriority = 50 + i;
                }
            }

            // 判断到达结束点 打开障碍组件
            for ( int i = 0; i < agents.Count; i++ )
            {
                if ( agents[i].enabled && agents[i].remainingDistance > 0.5f && agents[i].remainingDistance < 0.8f )
                {
                    agents[i].Stop();
                    agents[i].enabled = false;
                    agents[i].GetComponent<NavMeshObstacle>().enabled = true;
                }
            }
        }
    }
}
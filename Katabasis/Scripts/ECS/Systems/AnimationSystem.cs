using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System.Threading.Tasks;
using Unity.Mathematics;

public enum AnimationType
{
    PositionChange,
    Step,
    Flip,
    Butt,
    Flight,
    FallToTheAbyss,

}
[DisableAutoCreation]
public class AnimationSystem : ComponentSystem
{
    
    public static bool playAnimations;
    protected override void OnCreate()
    {

    }
    public static void AddAnimation(Entity entity, AnimationElement animationElement)
    {
        entity.GetBuffer<AnimationElement>().Add(animationElement);
        if(!entity.HasComponent<HasAnimationTag>())
        {
            entity.AddComponentData(new HasAnimationTag());
        }
    }
    public void ClearBuffers()
    {
        EntityQuery query = GetEntityQuery
        (
            ComponentType.ReadOnly<AnimationElement>()

        );

        Entities.With(query).ForEach((Entity entity, DynamicBuffer<AnimationElement> animationBuffer) =>
        {
            animationBuffer.Clear();
            entity.RemoveComponent<HasAnimationTag>();
        });
    }
    public async Task MinorUpdate()
    {
        bool atLeastOneAnimationIsPlaying = false;

        const int framesPerAnimation = 5;

        EntityQuery query = GetEntityQuery
        (
            ComponentType.ReadOnly<HasAnimationTag>()

        );

        Entities.With(query).ForEach(async (Entity entity, DynamicBuffer<AnimationElement> animationBuffer) =>
        {
            var trail = entity.GetComponentObject<EntityAuthoring>().trail;

            if (animationBuffer.Length > 1)
            {
                var transform = entity.GetComponentObject<Transform>();


                var currentAnimation = animationBuffer[animationBuffer.Length-1];

                if (Engine.i.showTrails && (currentAnimation.prevTile.visible || currentAnimation.nextTile.visible))
                {
                    trail.emitting = true;
                }

                if (currentAnimation.animationType != AnimationType.Butt
                    || currentAnimation.animationType != AnimationType.FallToTheAbyss)
                {
                    transform.position = currentAnimation.nextTile.position.ToRealPosition();
                }

                
            }
        });
    }
    public async Task UpdateAsync()
    {

        
        bool atLeastOneAnimationIsPlaying = false;
        
        const int framesPerAnimation = 5;

        EntityQuery query = GetEntityQuery
        (
            ComponentType.ReadOnly<HasAnimationTag>()

        );

        Entities.With(query).ForEach(async (Entity entity, DynamicBuffer<AnimationElement> animationBuffer)  => 
        {
            var trail = entity.GetComponentObject<EntityAuthoring>().trail;

            if (animationBuffer.Length > 1)
            {
                var transform = entity.GetComponentObject<Transform>();

                for (int i = 0; i < animationBuffer.Length - 1; i++)
                {
                    

                    var currentAnimation = animationBuffer[i];

                    if(Engine.i.showTrails && (currentAnimation.prevTile.visible|| currentAnimation.nextTile.visible))
                    {
                        trail.emitting = true;
                    }

                    if(currentAnimation.animationType != AnimationType.Butt
                        || currentAnimation.animationType != AnimationType.FallToTheAbyss)
                    {
                        transform.position = currentAnimation.nextTile.position.ToRealPosition();

                    }
                    
                }
            }
        });


        //draw last animation
        for (int i = 0; i < framesPerAnimation; i++)
        {
            query = GetEntityQuery
            (
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadWrite<HasAnimationTag>()
                
            );

            Entities.With(query).ForEach((Entity entity, DynamicBuffer<AnimationElement> animationBuffer ) =>
            {


                var animationComponent = animationBuffer[animationBuffer.Length - 1];
                var trail = entity.GetComponentObject<EntityAuthoring>().trail;
                var transform = entity.GetComponentObject<Transform>();
                    

                if(animationComponent.prevTile == TileData.Null)
                {
                    
                    animationComponent.nextTile = entity.CurrentTile(); 
                    animationComponent.prevTile = entity.CurrentTile();
                }


                if (Engine.i.playAnimations && animationComponent.animationType != AnimationType.PositionChange
                    && (animationComponent.prevTile.visible || animationComponent.nextTile.visible))
                {
                    //if animation needed
                    if(Engine.i.showTrails) trail.emitting = true;
                    var rendererTransform = transform.GetComponent<EntityAuthoring>().bodyRenderer.transform;
                    float2 vectorDistance = animationComponent.nextTile.position - animationComponent.prevTile.position;
                    float2 direction = math.normalize(vectorDistance);
                    float2 step = vectorDistance * new float2(1f / framesPerAnimation, 1f / framesPerAnimation);
                    float2 newObjectPosition = transform.position.ToFloat2();
                    float2 newRendererPosition = rendererTransform.position.ToFloat2();
                    Vector3 newRotation = Vector3.zero;
                    Vector3 newRendererScale = rendererTransform.localScale;

                    switch (animationComponent.animationType)
                    {
                        case AnimationType.Step:
                            newObjectPosition = transform.position.ToFloat2() + step;
                            newRendererPosition = newObjectPosition +
                                                    new float2(0, Engine.i.MovementAnimation.Evaluate(i));
                            break;

                        case AnimationType.Butt:

                            newRendererPosition = transform.position.ToFloat2() + direction * Engine.i.ButtAnimation.Evaluate(i);



                            break;
                        case AnimationType.Flip:
                            newObjectPosition = transform.position.ToFloat2() + step;
                            newRendererPosition = rendererTransform.position.ToFloat2() + step;
                            newRotation = new Vector3(0f, 0f, (360 / framesPerAnimation));

                            break;
                        case AnimationType.Flight:
                            newObjectPosition = transform.position.ToFloat2() + step;

                            break;
                        case AnimationType.FallToTheAbyss:
                            float d = newRendererScale.x - 0.1f;
                            newRendererScale = new Vector3(d, d, d);
                            newRendererPosition = newRendererPosition + new float2(0, -0.5f);
                            if (!entity.HasComponent<DestroyImmediatelyTag>())
                            {
                                entity.AddComponentData(new DestroyImmediatelyTag());
                                foreach (var item in entity.GetComponentData<AnatomyComponent>().GetLoverHierarchy())
                                {
                                    var renderer = item.GetComponentObject<RendererComponent>();
                                    renderer.spritesSortingLayerName = "Floor";
                                }
                            }
                            break;
                    }

                    transform.position = newObjectPosition.ToRealPosition();
                    rendererTransform.position = newRendererPosition.ToRealPosition();
                    rendererTransform.Rotate(newRotation);
                    rendererTransform.localScale = newRendererScale;


                    if (i + 1 == framesPerAnimation) //if this is last frame
                    {
                        rendererTransform.position = transform.position;
                        trail.emitting = false;
                        entity.RemoveComponent<HasAnimationTag>();
                        transform.position = entity.CurrentTile().position.ToRealPosition();

                    }
                    else
                    {
                        atLeastOneAnimationIsPlaying = true;
                    }

                }
                else 
                {
                    transform.position = entity.CurrentTile().position.ToRealPosition();
                    entity.RemoveComponent<HasAnimationTag>();
                }


            });

                if (atLeastOneAnimationIsPlaying)
                {
                    atLeastOneAnimationIsPlaying = false;
                    await Task.Delay(Engine.i.frameDrawInterval);
                }

        }

        playAnimations = false;

    }



    protected override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }

   


}
public struct HasAnimationTag : IComponentData
{

}
public struct AnimationElement : IBufferElementData
{
    //public Entity entity;
    public TileData prevTile;
    public TileData nextTile;
    public AnimationType animationType;
    public AnimationElement(TileData prevTile, TileData nextTile, AnimationType animationType)
    {
       // this.entity = entity;
        this.prevTile = prevTile;
        this.nextTile = nextTile;
        this.animationType = animationType;
    }
    public AnimationElement( TileData prevTile, AnimationType animationType)
    {
       // this.entity = entity;
        this.prevTile = prevTile;
        this.nextTile = prevTile;
        this.animationType = animationType;
    }
}




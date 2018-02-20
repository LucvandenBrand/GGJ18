import {Scene, WebGLRenderTarget, ConeGeometry, ShaderMaterial, Mesh,
        LinearFilter, NearestFilter} from 'three';
import voronoiVertex from './shaders/voronoiVertex.glsl'
import voronoiFragment from './shaders/voronoiFragment.glsl'


export default class VoronoiRenderer
{
  constructor(renderer, camera)
  {
    this.camera = camera;
    this.renderer = renderer;
    this.bufferTexture = new WebGLRenderTarget(renderer.getSize(),
                                               {minFilter: LinearFilter,
                                                magFilter: NearestFilter}
                                              );
    this.coneGeometry = new ConeGeometry(2, 2, 32);
    this.voronoiMaterial = 	new ShaderMaterial(
      {
        vertexShader   : voronoiVertex,
        fragmentShader : voronoiFragment
      });
  }

  /* Renders a voronoi diagram and returns it as a RenderTarget.*/
  render(players)
  {
    const voronoiScene = new Scene();
    players.forEach((player, index) => {
      const cone = new Mesh(coneGeometry, voronoiMaterial);
      cone.position = Vector3(player.position.x, player.position.y, 100);
      voronoiScene.add(cone);
    });
    renderer.render(voronoiScene, camera, bufferTexture);
    return bufferTexture;
  }
}

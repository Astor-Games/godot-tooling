// using GodotLib.Util;
//
// namespace GodotLib.Debug;
//
// [EntityRenderer(typeof(Relationship<>))]
// public class RelationshipRenderer<T> : IEntityTreeRenderer
// {
//     public void Render(TreeItem rootItem, object component, RenderingParameters parameters)
//     {
//         var relationship = (Relationship<T>)component;
//         var relType = typeof(T);
//         var relName = relType.GetHumanReadableName();
//         
//         rootItem.SetText(0, relName);
//         
//         var i = 0;
//         
//         foreach (var (entity, rel) in relationship)
//         {
//             var item = EntityTreeRendering.Render(rootItem, i++, entity, i.ToString(), parameters);
//             EntityTreeRendering.RenderFields(item, rel, parameters);
//         }
//     }
// }
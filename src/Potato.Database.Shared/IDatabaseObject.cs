#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using Potato.Database.Shared.Builders;

namespace Potato.Database.Shared {
    /// <summary>
    /// The base object with a bunch of helper methods to make
    /// building a query relatively 
    /// </summary>
    public interface IDatabaseObject : ICollection<IDatabaseObject> {

        /// <summary>
        /// Append a Method type to the list of objects on this
        /// </summary>
        /// <param name="data">A method object</param>
        /// <returns>this</returns>
        IDatabaseObject Method(IDatabaseObject data);

        /// <summary>
        /// Appends a Database object to the list of objects on this
        /// </summary>
        /// <param name="data">A database object</param>
        /// <returns>this</returns>
        IDatabaseObject Database(IDatabaseObject data);

        /// <summary>
        /// Creates a new Database object with a specified name, then
        /// appends the object ot the list of objects on this
        /// </summary>
        /// <param name="name">The name of the database</param>
        /// <returns>this</returns>
        IDatabaseObject Database(string name);

        /// <summary>
        /// Append a Index type to the list of objects on this
        /// </summary>
        /// <param name="data">The index to append</param>
        /// <returns>this</returns>
        IDatabaseObject Index(IDatabaseObject data);

        /// <summary>
        /// Creates a new ascending index on a field with name
        /// </summary>
        /// <param name="collection">The name of the collectionthe field with name exists in</param>
        /// <param name="name">The name of the field to index on</param>
        /// <returns>this</returns>
        IDatabaseObject Index(string collection, string name);

        /// <summary>
        /// Creates a new ascending or descending index on a field with name
        /// </summary>
        /// <param name="collection">The name of the collectionthe field with name exists in</param>
        /// <param name="name">The name of the field to index on</param>
        /// <param name="sortByModifier">The direction to sort by</param>
        /// <returns>this</returns>
        /// <remarks>Passing Ascending through is as good as using Index(name)</remarks>
        IDatabaseObject Index(string collection, string name, ISortByModifier sortByModifier);

        /// <summary>
        /// Creates a new ascending index on a field with name, potentially a unique or primary key
        /// </summary>
        /// <param name="collection">The name of the collectionthe field with name exists in</param>
        /// <param name="name">The name of the field to index on</param>
        /// <param name="indexModifier">The type of index (primary, unique etc.)</param>
        /// <returns>this</returns>
        IDatabaseObject Index(string collection, string name, IIndexModifier indexModifier);

        /// <summary>
        /// Creates a new ascending or descenting index on a field with name, potentially a unique or primary key
        /// </summary>
        /// <param name="collection">The name of the collectionthe field with name exists in</param>
        /// <param name="name">The name of the field to index on</param>
        /// <param name="indexModifier">The type of index (primary, unique etc.)</param>
        /// <param name="sortByModifier">The direction to sort by</param>
        /// <returns>this</returns>
        IDatabaseObject Index(string collection, string name, IIndexModifier indexModifier, ISortByModifier sortByModifier);

        /// <summary>
        /// Appends a modifier
        /// </summary>
        /// <param name="data">The modifier to append</param>
        /// <returns>this</returns>
        IDatabaseObject Modifier(IDatabaseObject data);

        /// <summary>
        /// Appends a field type
        /// </summary>
        /// <param name="data">The field type to append</param>
        /// <returns>this</returns>
        IDatabaseObject FieldType(IDatabaseObject data);

        /// <summary>
        /// Appends a field object to the chain
        /// </summary>
        /// <param name="data">The field objec to append</param>
        /// <returns>this</returns>
        IDatabaseObject Field(IDatabaseObject data);

        /// <summary>
        /// Creates and appends new field object with a field name of `name`
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <returns>this</returns>
        IDatabaseObject Field(string name);

        /// <summary>
        /// Creates and appends a new field object with name of `name`, a specified type and if the field is nullable or not
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="type">The type of field to use</param>
        /// <param name="nullable">true if the field allows null</param>
        /// <returns>this</returns>
        IDatabaseObject Field(string name, IFieldType type, bool nullable = true);

        /// <summary>
        /// Creates and appends a new field object with name of `name`, a string type with a specified length and if the field is nullable or not
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <param name="length">The maximum length of the field data</param>
        /// <param name="nullable">true if the field allows null</param>
        /// <returns>this</returns>
        IDatabaseObject Field(string name, int length, bool nullable = true);

        /// <summary>
        /// Appends an equality object
        /// </summary>
        /// <param name="data">The equality object to append</param>
        /// <returns>this</returns>
        IDatabaseObject Condition(IDatabaseObject data);

        /// <summary>
        /// Creates a new condition object, a field with name equaling data
        /// </summary>
        /// <param name="name">The name of the field to compare against</param>
        /// <param name="data">The data to compare against</param>
        /// <returns>this</returns>
        IDatabaseObject Condition(string name, object data);

        /// <summary>
        /// Create a new condition object, field with name, a comparison of equality
        /// against data
        /// </summary>
        /// <param name="name">The name of the field to compare against</param>
        /// <param name="equality">The equality to compare the field against the data</param>
        /// <param name="data">The data to compare against</param>
        /// <returns>this</returns>
        IDatabaseObject Condition(string name, IEquality equality, object data);

        /// <summary>
        /// Creates a new condition object, a field with name equaling data
        /// </summary>
        /// <param name="collection">The specific collection to prefix the field with e.g "Player.Name"</param>
        /// <param name="name">The name of the field to compare against</param>
        /// <param name="data">The data to compare against</param>
        /// <returns>this</returns>
        IDatabaseObject Condition(string collection, string name, object data);

        /// <summary>
        /// Create a new condition object, field with name, a comparison of equality
        /// against data
        /// </summary>
        /// <param name="collection">The specific collection to prefix the field with e.g "Player.Name"</param>
        /// <param name="name">The name of the field to compare against</param>
        /// <param name="equality">The equality to compare the field against the data</param>
        /// <param name="data">The data to compare against</param>
        /// <returns>this</returns>
        IDatabaseObject Condition(string collection, string name, IEquality equality, object data);

        /// <summary>
        /// Appends an assignment object to the chain
        /// </summary>
        /// <param name="data">The assignment object</param>
        /// <returns>this</returns>
        IDatabaseObject Set(IDatabaseObject data);

        /// <summary>
        /// Creates a new assignment object to set a field with `name` to `data`
        /// </summary>
        /// <param name="name">The field to set</param>
        /// <param name="data">The data to set the field as</param>
        /// <returns>this</returns>
        IDatabaseObject Set(string name, object data);
        
        /// <summary>
        /// Appends a collection object to the chain
        /// </summary>
        /// <param name="data">The collection object</param>
        /// <returns>this</returns>
        IDatabaseObject Collection(IDatabaseObject data);

        /// <summary>
        /// Creates a new collection object, then appends to the list
        /// </summary>
        /// <param name="name">The name of the table or collection</param>
        /// <returns>this</returns>
        IDatabaseObject Collection(string name);

        /// <summary>
        /// Appends a sort object to the chain
        /// </summary>
        /// <param name="data">The sort object</param>
        /// <returns>this</returns>
        IDatabaseObject Sort(IDatabaseObject data);

        /// <summary>
        /// Creates a new sort object, then appends to the list
        /// </summary>
        /// <param name="name">The name of the field to sort</param>
        /// <param name="modifier">Ascending or descending, which appends a Ascending order modifier if null (default) </param>
        /// <returns>this</returns>
        IDatabaseObject Sort(string name, ISortByModifier modifier = null);

        /// <summary>
        /// Creates a new sort object, then appends to the list
        /// </summary>
        /// <param name="collection">The collection the field exists in to sort</param>
        /// <param name="name">The name of the field to sort</param>
        /// <param name="modifier">Ascending or descending, which appends a Ascending order modifier if null (default) </param>
        /// <returns>this</returns>
        IDatabaseObject Sort(string collection, string name, ISortByModifier modifier = null);

        /// <summary>
        /// Appends a limit modifer to the chain
        /// </summary>
        /// <param name="data">The limit object</param>
        /// <returns>this</returns>
        IDatabaseObject Limit(IDatabaseObject data);

        /// <summary>
        /// Creates a new limit modifier and appends it to the chain
        /// </summary>
        /// <param name="limit">The value to place inside the limit</param>
        /// <returns>this</returns>
        IDatabaseObject Limit(int limit);

        /// <summary>
        /// Appends a skip modifer to the chain
        /// </summary>
        /// <param name="data">The skip object</param>
        /// <returns>this</returns>
        IDatabaseObject Skip(IDatabaseObject data);

        /// <summary>
        /// Creates a new skip modifier and appends it to the chain
        /// </summary>
        /// <param name="skip">The value to place inside the skip object</param>
        /// <returns>this</returns>
        IDatabaseObject Skip(int skip);

        /// <summary>
        /// Tags this object with a implicit modifier, letting the serializer know that it has been automatically generated
        /// while using shorthand methods.
        /// </summary>
        /// <returns>this</returns>
        IDatabaseObject Implicit();

        /// <summary>
        /// Tags the object with an explicit modifier, letting the serializer know it has been explicitly appended
        /// to the query chain and not generated bybuilder code.
        /// </summary>
        /// <returns>this</returns>
        IDatabaseObject Explicit();

        /// <summary>
        /// Appends a database object without any modifications, returns the current object.
        /// </summary>
        /// <param name="item">The item to append to the chain</param>
        /// <returns>this</returns>
        IDatabaseObject Raw(IDatabaseObject item);
    }
}

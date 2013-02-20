//
//  P31MonoBridge.h
//  Unity-iPhone
//
//  Created by Mike on 5/19/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>


// Mono goodies
extern "C"
{
	typedef void* MonoDomain;
	typedef void* MonoAssembly;
	typedef void* MonoImage;
	typedef void* MonoClass;
	typedef void* MonoObject;
	typedef void* MonoMethodDesc;
	typedef void* MonoMethod;
	typedef void* MonoString;
	//typedef void* MonoArray;
	typedef char MonoBoolean;
	typedef void* MonoEvent;
	typedef void* gpointer;
	typedef int gboolean;
	typedef int64_t mono_array_size;
	typedef int64_t mono_array_size_t;
	
	typedef struct {
		mono_array_size_t length;
		mono_array_size_t lower_bound;
	} MonoArrayBounds;
	
	typedef struct {
		MonoObject obj;
		/* bounds is NULL for szarrays */
		MonoArrayBounds *bounds;
		/* total number of elements of the array */
		mono_array_size_t max_length; 
		/* we use double to ensure proper alignment on platforms that need it */
		double vector [0];
	} MonoArray;
	
#define mono_array_addr(array,type,index) ((type*)(gpointer) mono_array_addr_with_size (array, sizeof (type), index))
#define mono_array_addr_with_size(array,size,index) ( ((char*)(array)->vector) + (size) * (index) )
	
#define mono_array_set(array,type,index,value)	\
do {	\
type *__p = (type *)mono_array_addr((array), type, (index));	\
*__p = (value);	\
} while (0)
	
	
	MonoDomain *mono_domain_get();
	MonoDomain *mono_get_root_domain();
	void mono_thread_attach( MonoDomain *domain );
	MonoDomain *mono_jit_init( const char *file );
	
	MonoAssembly *mono_domain_assembly_open( MonoDomain *domain, const char *assemblyName );
	MonoImage *mono_assembly_get_image( MonoAssembly *assembly );
	MonoMethodDesc *mono_method_desc_new( const char *methodString, gboolean useNamespace );
	void mono_method_desc_free( MonoMethodDesc *desc );
	MonoMethod *mono_method_desc_search_in_image( MonoMethodDesc *methodDesc, MonoImage *image );
	MonoObject *mono_runtime_invoke( MonoMethod *method, void *obj, void **params, MonoObject **exc );
	MonoString *mono_string_new( MonoDomain *domain, const char *cString );
	
	MonoClass *mono_class_from_name( MonoImage *image, const char *namespaceString, const char *classnameString );
	MonoMethod *mono_class_get_methods( MonoClass*, gpointer* iter );
	
	void* mono_object_unbox( MonoObject* );
	char* mono_string_to_utf8( MonoString* );
	MonoArray* mono_array_new( MonoDomain*, MonoClass*, mono_array_size );
	MonoClass* mono_get_byte_class();
}


@interface P31MonoBridge : NSObject
{
@private
	MonoDomain *monoDomain;
	MonoImage *monoFirstpassImage;
	MonoImage *monoCsharpImage;
	NSMutableDictionary *methodCache;
}
@property (nonatomic, retain) NSMutableDictionary *methodCache;


+ (P31MonoBridge*)sharedBridge;


- (MonoMethod*)monoMethodForStaticUnityMethod:(NSString*)unityMethod;

// Shortcuts for calling mono methods
- (void)callMonoMethod:(MonoMethod*)method;

- (void)callMonoMethod:(MonoMethod*)method withInt:(int)param;

- (void)callMonoMethod:(MonoMethod*)method withString:(NSString*)param;

- (void)callMonoMethod:(MonoMethod*)method withArgs:(void*)args;

// with returns
- (int)callMonoMethodWithIntReturn:(MonoMethod*)method;

- (BOOL)callMonoMethodWithBoolReturn:(MonoMethod*)method;

- (NSString*)callMonoMethodWithStringReturn:(MonoMethod*)method;


// param helers
- (MonoString*)monoStringParam:(NSString*)string;

- (MonoArray*)monoArrayParamForByteArray:(NSData*)data;

@end
